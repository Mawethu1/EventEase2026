@echo off
az sql server firewall-rule create --resource-group VenueBookingRG --server venuebooking-sqlsrv-westus --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 --output json
echo done
