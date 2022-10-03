# Choose some resource names. Note that some of these are globally unique across all of Azure, so you will need to change them
$resourceGroup = "orleans-stock-exchange"
$location = "germanywestcentral"
$clusterName = "orleans-stock-exchange-aks"
$containerRegistry = "orleans-stock-exchange-acr"

#az login

# Create a resource group
#az group create --name $resourceGroup --location $location

# Create an AKS cluster. This can take a few minutes
az aks create --generate-ssh-keys --resource-group $resourceGroup --name $clusterName --node-count 3

# Authenticate the Kubernetes CLI
az aks get-credentials --resource-group $resourceGroup --name $clusterName

# Create an Azure Container Registry account and login to it
az acr create --name $containerRegistry --resource-group $resourceGroup --sku Standard

# Create a service principal for the container registry and register it with Kubernetes as an image pulling secret
$acrId = $(az acr show --name $containerRegistry --query id --output tsv)
$acrServicePrincipalName = "$($containerRegistry)-aks-service-principal"
$acrSpPw = $(az ad sp create-for-rbac --name http://$acrServicePrincipalName --scopes $acrId --role acrpull --query password --output tsv)
$acrSpAppId = $(az ad sp show --id http://$acrServicePrincipalName --query appId --output tsv)
$acrLoginServer = $(az acr show --name $containerRegistry --resource-group $resourceGroup --query loginServer).Trim('"')
kubectl create secret docker-registry $containerRegistry --namespace default --docker-server=$acrLoginServer --docker-username=$acrSpAppId --docker-password=$acrSpPw