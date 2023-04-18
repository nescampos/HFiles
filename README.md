# HFiles

HFiles is a decentralized files manager connected to the Hedera network.

This project use [Hashgraph](https://bugbytesinc.github.io/Hashgraph/)


## License
----
Apache-2.0 License
 
## Contributors
----

- [Néstor Nicolás Campos Rojas](https://www.linkedin.com/in/nescampos/)


## Use this project

First, you need to update the database applying the available migration in the project:
```sh
    dotnet ef database update
```

To consume this project, just run with Visual Studio or DotNet CLI in your project.

```sh
    dotnet run
```

*This project is built with .NET 6.0*

## Configuration

You need to edit the **appsettings.json** file for adding:
- DefaultConnection: A SQL Server connection (use for saving info about the projects).
- HederaNetwork: The URL for Hedera gateway (testing or production).
- HederaNodeAccountId: The node number for Hedera gateway (0,1,2,3,4,5, etc.).
- HederaAccountId: The Hedera Account Id for govern this app (send transactions, create tokens, create Hedera test accounts, etc.).
- HederaPublicKey: The Hedera Public Key for govern this app (send transactions, create tokens, create Hedera test accounts, etc.).
- HederaPrivateKey: The Hedera Private Key for govern this app (send transactions, create tokens, create Hedera test accounts, etc.).

## Contributions

If you want to colaborate, just fork this repository and build new things. Thanks!!
