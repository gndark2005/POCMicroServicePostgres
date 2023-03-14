import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildFeatures.PullRequests
import jetbrains.buildServer.configs.kotlin.buildFeatures.commitStatusPublisher
import jetbrains.buildServer.configs.kotlin.buildFeatures.dockerSupport
import jetbrains.buildServer.configs.kotlin.buildFeatures.pullRequests
import jetbrains.buildServer.configs.kotlin.buildSteps.dockerCommand
import jetbrains.buildServer.configs.kotlin.buildSteps.script
import jetbrains.buildServer.configs.kotlin.triggers.vcs

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2022.04"

project {

    buildType(BuildAndTestPullRequests)
    buildType(BuildAndTestMasterBranch)

    params {
        param("DockerImage", "ghcr.io/buildinglink/buildagent-dotnet:3.1")
        param("ContainerTag", "%build.number%")
        param("env.SONAR_PROJECT", "BuildingLink_netcore-bms-template")
        param("ContainerName", "ghcr.io/buildinglink/bms/template")
    }
    buildTypesOrder = arrayListOf(BuildAndTestPullRequests, BuildAndTestMasterBranch)
}

object BuildAndTestMasterBranch : BuildType({
    name = "Build and Test Master Branch"

    allowExternalStatus = true
    enablePersonalBuilds = false
    artifactRules = """
        dotCover => dotCover.zip!dotCover
        dotCover.html => dotCover.zip
    """.trimIndent()
    buildNumberPattern = "1.0.%build.counter%"
    maxRunningBuilds = 1
    publishArtifacts = PublishMode.SUCCESSFUL

    params {
        param("env.SONAR_ARGS", "/o:buildinglink /d:sonar.qualitygate.wait=true /v:%build.number%")
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = "+:<default>"
    }

    steps {
        script {
            name = "Build and Test in a Container"
            scriptContent = """
                # generate a unique name to avoid conflicts when we run a few builds on the same build agent
                COMPOSE_PROJECT_NAME=${'$'}(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 8 | head -n 1)
                
                docker-compose -p ${'$'}{COMPOSE_PROJECT_NAME} -f "tests/docker-compose.tests.yml" up --force-recreate --abort-on-container-exit --build --exit-code-from integration
                
                DC_EXIT_CODE=${'$'}?
                
                docker-compose -p ${'$'}{COMPOSE_PROJECT_NAME} -f "tests/docker-compose.tests.yml" down -v --rmi all
                
                exit ${'$'}DC_EXIT_CODE
            """.trimIndent()
        }
        dockerCommand {
            name = "Build Docker Image"
            commandType = build {
                source = file {
                    path = "Dockerfile"
                }
                contextDir = "./"
                namesAndTags = """
                    %ContainerName%:%ContainerTag%
                    %ContainerName%:latest
                """.trimIndent()
                commandArgs = "--build-arg GITHUB_TOKEN=%env.GITHUB_TOKEN%"
            }
        }
        dockerCommand {
            name = "Push Docker Image"
            enabled = false
            commandType = push {
                namesAndTags = """
                    %ContainerName%:%ContainerTag%
                    %ContainerName%:latest
                """.trimIndent()
            }
        }
    }

    triggers {
        vcs {
            branchFilter = "+:<default>"
        }
    }

    features {
        dockerSupport {
            loginToRegistry = on {
                dockerRegistryId = "PROJECT_EXT_19"
            }
        }
    }
})

object BuildAndTestPullRequests : BuildType({
    name = "Build and Test (pull requests)"

    artifactRules = """
        dotCover => dotCover.zip!dotCover
        dotCover.html => dotCover.zip
    """.trimIndent()
    publishArtifacts = PublishMode.SUCCESSFUL

    params {
        param("teamcity.git.fetchAllHeads", "true")
        param("env.SONAR_ARGS", "/o:buildinglink /d:sonar.pullrequest.key=%teamcity.pullRequest.number% /d:sonar.pullrequest.branch=%teamcity.build.branch% /d:sonar.pullrequest.base=master /d:sonar.pullrequest.provider=github /d:sonar.pullrequest.github.repository=BuildingLink/netcore-bms-template")
    }

    vcs {
        root(DslContext.settingsRoot)

        cleanCheckout = true
        branchFilter = "+:pull/*"
        excludeDefaultBranchChanges = true
    }

    steps {
        script {
            name = "Build and Test in a Container"
            scriptContent = """
                # generate a unique name to avoid conflicts when we run a few builds on the same build agent
                COMPOSE_PROJECT_NAME=${'$'}(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 8 | head -n 1)
                
                docker-compose -p ${'$'}{COMPOSE_PROJECT_NAME} -f "tests/docker-compose.tests.yml" up --force-recreate --abort-on-container-exit --build --exit-code-from integration
                
                DC_EXIT_CODE=${'$'}?
                
                docker-compose -p ${'$'}{COMPOSE_PROJECT_NAME} -f "tests/docker-compose.tests.yml" down -v --rmi all
                
                exit ${'$'}DC_EXIT_CODE
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            branchFilter = "+:pull/*"
        }
    }

    features {
        pullRequests {
            provider = github {
                authType = token {
                    token = "credentialsJSON:e85b2efa-a9d8-4223-b0a6-8729ae1a5b64"
                }
                filterAuthorRole = PullRequests.GitHubRoleFilter.EVERYBODY
            }
        }
        commitStatusPublisher {
            publisher = github {
                githubUrl = "https://api.github.com"
                authType = personalToken {
                    token = "credentialsJSON:e85b2efa-a9d8-4223-b0a6-8729ae1a5b64"
                }
            }
        }
        dockerSupport {
            loginToRegistry = on {
                dockerRegistryId = "PROJECT_EXT_19"
            }
        }
    }
})
