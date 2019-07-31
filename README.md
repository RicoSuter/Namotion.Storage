# Namotion.Storage

[![Azure DevOps](https://img.shields.io/azure-devops/build/rsuter/Namotion/19/master.svg)](https://rsuter.visualstudio.com/Namotion/_build?definitionId=19)

The Namotion.Storage .NET libraries provide abstractions and implementations for storage service like blob storages or file systems.

By programming against a messaging abstraction you enable the following scenarios: 

- Build **multi-cloud capable applications** by being able to change storage technologies on demand. 
- Quickly **switch to different storage technologies** to find the best technological fit for your applications. 
- Implement behavior driven **integration tests which can run in-memory** or against different technologies for better debugging experiences or local execution. 
- Provide **better local development experiences**, e.g. replace Azure Blob Storage with the local file system or an in-memory implementation. 

## Usage

TODO

## Extensions

TODO

## Core packages

### Namotion.Storage.Abstractions

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Abstractions.svg)](https://www.nuget.org/packages/Namotion.Storage.Abstractions/)

**Blobs**

- **BlobProperties
\<T>**
- **IBlobStorage**: A blob storage where blobs are stored in a container and cannot be directly stored. Only 'containerName/blobName' or 'containerName/subDirectories/blobName' are allowed.
- **IBlobContainer\<T>**
- **IBlobContainer**: A blob container where blobs can be directly stored or in a subdirectory.
- **BlobElement**: Metadata and properties of a blob or container.
- **IBlobReader**: Internal (do not use directly.)
- **IBlobWriter**: Internal (do not use directly.)

### Namotion.Storage.Json

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Json.svg)](https://www.nuget.org/packages/Namotion.Storage.Json/)

TODO

## Implementation packages

The following packages should only be used in the head project, i.e. directly in your application bootstrapping project where the dependency injection container is initialized.

TODO

### Namotion.Storage

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.svg)](https://www.nuget.org/packages/Namotion.Storage/)

TODO

### Namotion.Storage.Azure.Storage.Blob

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Azure.Storage.Blob.svg)](https://www.nuget.org/packages/Namotion.Storage.Azure.Storage.Blob/)

TODO
