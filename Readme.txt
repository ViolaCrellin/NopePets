About
REST API using WCF based on Neopets a website to which I lost a lot of my time growing up and tickles my nostalgia bone. 
Except, for legal reasons, it is called NopePets, and partly as a hommage to a colleague who pronounces 'Nope' as 'NeeeoP'

The technology stack chosen was more regarding what represented more of a learning opportunity than anything else. There are probably much more suitable frameworks to choose but I just felt like learning something different.

Where a personal implementation has been made (such as my own containers) over using something pre-packaged is largely for similar reasons. 

Dependencies
Pre-requisites are a 2016+ version of SQL Server - the .mdf files are version 852 and no earlier versions are supported.

Design
The project aims to be ultimately extensible, even if this has caused cases of YAGNI. 
A good basis for extensibility is a properly normalised database schema (as this can be the hardest thing to change), from which most of the design grew out of. 

The project is arranged around 3 mirroring services.
 
Site Service - responsible for non-user specific requests
User Session Service - responsible for handling any user-specific requests
Game Service - responsible for game data and likely to be extended to be an interface intended only for game makers and not game players.

The services are not fully separated and talking to each other as of yet, but scope is there to do so along with sharding the database appropriately. 

Each service has corresponding:
State class
Request processor
Validators
Container module from which to resolve resources shared across the services (eg Repositories, RecordPersisters and DataProviders)

DTOs are Request<T> and Response<T>, where T reflects the data requested by the client or returned by the server. 

Database modelled objects are translated to more client-friendly ones via the ResponseBuilder, which is comprised of many separate builders that translate the DB objects according to business logic. 

Usually data is more easily converted back to database row objects and so a single Converter class suffices.

Todos
Update User and Pet information (such as names)
Query for specific game data (ie What are a Pet rock's metrics?) or user session data
Delete Pets (or kill them off if they've been neglected for too long!)

Administrator login to be able to amend game data via the API
CRUD Game data via the API (So add your own Animals and their health stats)

