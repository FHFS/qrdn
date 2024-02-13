# Local machine
## Build
 ```
 dotnet build
```
## Run
```
dotnet run
```


# Docker
## Build
 ```
 docker build -t qrdn:latest -f ./Dockerfile --network=host .
```
Give the image a name and version with `-t qrdn:latest`
Specify the Dockerfile with `-f ./Dockerfile` (Optional)
Use the network of the hostmachine with ` --network=host` if the docker build is unable to resolve urls.
Specify the current directory as context with `.`
## Run
```
docker run --network=host qrdn:latest
docker run -p 8080:80 qrdn:latest
```