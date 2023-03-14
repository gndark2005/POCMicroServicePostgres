param(
     [parameter(Mandatory=$true)] [string]$api_serviceName = "bl-api-mgmt-dev",
     [parameter(Mandatory=$true)] [string]$api_id = "template-api",
     [parameter(Mandatory=$true)] [string]$api_name = "Template-PropertyEmployee-v1",
     [parameter(Mandatory=$true)] [string]$api_products = "Internal Staff APIs, Internal Website APIs",
     [parameter(Mandatory=$true)] [string]$api_scopes_read = "template_read, internal_staff_app_apis, internal_web_apis",
     [parameter(Mandatory=$true)] [string]$api_scopes_write = "template_write, internal_staff_app_apis internal_web_apis",
     [parameter(Mandatory=$true)] [string]$backend_service_url = "https://blamenitiesstaging.azurewebsites.net/",
     [parameter(Mandatory=$true)] [string]$api_url_suffix = "TemplateService/PropertyEmployee/v1",
     [parameter(Mandatory=$true)] [string]$api_swagger_endpoint = "/swagger/v1/swagger.json",
     [parameter(Mandatory=$true)] [string]$clientSecret = "",
     [parameter(Mandatory=$true)] [string]$azureAppId = "",
     [parameter(Mandatory=$true)] [string]$tenant = ""
     
 );

 $DebugPreference = "Continue"

 Write-Host "####################################"
 Write-Host "Azure Service Name: " $api_serviceName;
 Write-Host "API Id: " $api_id;
 Write-Host "API Name: " $api_name;
 Write-Host "API Products: " $api_products;
 Write-Host "API Scopes Read: " $api_scopes_read;
 Write-Host "API Scopes Write: " $api_scopes_write;
 Write-Host "Azure Backend Service URL: " $backend_service_url;
 Write-Host "Azure API Url Suffix: " $api_url_suffix;
 Write-Host "Swagger Endpoint: " $api_swagger_endpoint;
 Write-Host "####################################"
function ImportModulesAndAddAssemblies()
 {
    Write-Host "Azure modules loading...";
    
    if ( Get-InstalledModule Az)
    {
        Write-Host "Az module loaded.";
    }
    else {
        Write-Host "Installing Az module.";
        Install-Module -Name Az -AllowClobber -Scope CurrentUser -Force
    }
    
    Import-Module -Name Az.Accounts 
    
    Write-Host "Azure modules loaded successfully";
 }

function GetAzureContext($userInfo)
 {      
    Write-Host "Get Azure Context...";

    $securePassword = ConvertTo-SecureString $clientSecret -AsPlainText -Force;
    $credentials = (New-Object System.Management.Automation.PSCredential $azureAppId, $securePassword )
    $azureAccount = Connect-AzAccount -Credential $credentials -Tenant $tenant -ServicePrincipal
    if($azureAccount){
       $azureContext = New-AzApiManagementContext -ResourceGroupName "Api-Default-East-US" -ServiceName $api_serviceName;

        Write-Host "Get Azure Context finished.";

        return $azureContext;
    }
    else{
        throw "Azure Ad Authentication Failed";        
        #[Environment]::Exit(1);
    }
 }

 function DeployApiToAzureGateway($sessionContext) {
    Write-Host "Create/Update API module";

    ## get swagger
    $swaggerUrl = $backend_service_url + $api_swagger_endpoint;

    $api = Import-AzApiManagementApi -Context $sessionContext -ApiId $api_id -SpecificationFormat OpenApiJson -SpecificationUrl $swaggerUrl -Path $api_url_suffix -ErrorAction Ignore -ErrorVariable +azureImportError;
    
    Write-Host  "Imported API: " $api

    if ($azureImportError) {
        Write-Host "##[warning] " $azureImportError[0].Exception.Error.OriginalMessage;
        $failed++;
        $azureImportError = $null;
    }
    else {
        # update web service Root Url with ApiId "template-api"
        Set-AzApiManagementApi -Context $context -ApiId $api_id -Name $api_name -Protocols @('https') -ServiceUrl $backend_service_url  -Description $api_products -ErrorAction Ignore -ErrorVariable +azureImportError;# -Description "Responds with what was sent" -Path "echo"

        if ($azureImportError) {
            Write-Host "##[warning] " $azureImportError[0].Exception.Error.OriginalMessage;            
        }
        else {
            Write-Host "##[info] Create/Update module completed"; 
        }
    }    
}

ImportModulesAndAddAssemblies;
$context = GetAzureContext ($userName, $password);
DeployApiToAzureGateway ($context);

# force to exit success
exit 0
