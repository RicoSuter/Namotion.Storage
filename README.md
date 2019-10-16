# Namotion.Storage

Storage | [Messaging](https://github.com/RicoSuter/Namotion.Messaging) | [Reflection](https://github.com/RicoSuter/Namotion.Reflection)

[![Azure DevOps](https://img.shields.io/azure-devops/build/rsuter/9023bd0a-b641-4e30-9c0f-a7c15e1e080e/20/master.svg)](https://dev.azure.com/rsuter/Namotion/_build?definitionId=20)
[![Azure DevOps](https://img.shields.io/azure-devops/coverage/rsuter/9023bd0a-b641-4e30-9c0f-a7c15e1e080e/20/master.svg)](https://dev.azure.com/rsuter/Namotion/_build?definitionId=20)

<img align="left" src="https://raw.githubusercontent.com/RicoSuter/Namotion.Reflection/master/assets/Icon.png" width="48px" height="48px">

The Namotion.Storage .NET libraries provide abstractions and implementations for storage services like blob storages, file systems or object storages.

By programming against a storage abstraction you enable the following scenarios: 

- Build **multi-cloud capable applications** by being able to change storage technologies on demand. 
- Quickly **switch to different storage technologies** to find the best technological fit for your applications. 
- Implement behavior driven **integration tests which can run in-memory** or against different technologies for better debugging experiences or local execution. 
- Provide **better local development experiences**, e.g. replace Azure Blob Storage with the local file system or an in-memory implementation. 

## Usage

In your application root, create an `IBlobStorage` instance with an actual implementation package and retrieve a blob container: 

```csharp
var storage = AzureBlobStorage.CreateFromConnectionString("MyConnectionString");
IBlobContainer container = storage.GetContainer("MyContainer");
IBlobContainer<Person> typedContainer = container.WithBlobType<Person>();

await typedContainer.WriteAsJsonAsync("MyPath", new Person { ... });
var person = await typedContainer.ReadAsJsonAsync("MyPath");
```

In your business service classes you should then only use the abstraction interfaces like `IBlobContainer` or `IObjectStorage`, etc.

## Core packages

### Namotion.Storage.Abstractions

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Abstractions.svg)](https://www.nuget.org/packages/Namotion.Storage.Abstractions/)

**Blobs**

Inject `IBlobStorage` or `IBlobContainer` but do not get a container from a blob storage in the consuming class (violates [SRP](http://software-pattern.org/single-responsibility-principle)).

- **IBlobStorage**: A blob storage where blobs are stored in a container and cannot be directly stored. Only `containerName/blobName` or `containerName/subDirectories/blobName` are allowed.
- **IBlobContainer\<T>**
- **IBlobContainer**: A blob container where blobs can be directly stored or in a subdirectory. A container acts like a simple/basic virtual file system.
    - `OpenWriteAsync`: Creates or overrides an existing blob
    - `OpenAppendAsync`: Creates or appends to an existing blob
    - `OpenReadAsync`
    - `ExistsAsync`
    - `GetAsync`
    - `ListAsync`
    - `DeleteAsync`: Deletes a blob
- **BlobElement**: Metadata and properties of a blob or container.

Internal: 

- **IBlobReader**: Internal (do not use directly.)
- **IBlobWriter**: Internal (do not use directly.)

The idea behind the generic interfaces is to allow multiple instance registrations, read [Dependency Injection in .NET: A way to work around missing named registrations](https://blog.rsuter.com/dotnet-dependency-injection-way-to-work-around-missing-named-registrations/) for more information.

**Objects**

- **IObjectStorage\<T>**
    - `WriteAsync(id, value)`
    - `ReadAsync(id)`
    - `DeleteAsync(id)`

### Namotion.Storage.Json

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Json.svg)](https://www.nuget.org/packages/Namotion.Storage.Json/)

Extension methods:

- `WriteAsJson()`: Writes an object as JSON into a blob container/storage.
- `ReadAsJson()`: Reads an object as JSON from a blob container/storage.
- `CreateJsonObjectStorage<T>()`: Creates an `IObjectStorage<T>` for a given `IBlobContainer`. Usage: `var objectStorage = blobContainer.CreateJsonObjectStorage<Person>()`.

## Implementation packages

The following packages should only be used in the head project, i.e. directly in your application bootstrapping project where the dependency injection container is initialized.

### Namotion.Storage

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.svg)](https://www.nuget.org/packages/Namotion.Storage/)

Implementations:

- FileSystemBlobStorage
- InMemoryBlobStorage

### Namotion.Storage.Azure.Storage.Blob

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Azure.Storage.Blob.svg)](https://www.nuget.org/packages/Namotion.Storage.Azure.Storage.Blob/)

Implementations:

- AzureBlobStorage

### Namotion.Storage.Ftp

[![Nuget](https://img.shields.io/nuget/v/Namotion.Storage.Ftp.svg)](https://www.nuget.org/packages/Namotion.Storage.Ftp.Blob/)

Implementations:

- FtpBlobStorage
