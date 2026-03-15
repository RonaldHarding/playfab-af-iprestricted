# Disclaimer

This is not an official PlayFab sample. The code in this repo is provided as is and carries no guarantee or support. Feel free to sample, copy, transform, or redistribute this code for any purpose but at your own risk. Running the provided bicep file against your existing function app may result in destructive changes.

# Description

This is a demonstration of a PlayFab integrated Azure Function which is secured against outside traffic. It utilizes Azure Function IP rules to restrict inbound traffic to be from the known PlayFab IP range and checks the title Id on incoming requests matches the configured title the function is meant to be used with. This protects the application by not allowing attacks from unauthorized clients, and blocking unexpected traffic that uses a different PlayFab title as a proxy.

# Configuration

main.bicepparam contains the allowed title id's, the allowed ip addresses, and the function app name. Please update these values as needed.