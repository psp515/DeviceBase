<div align="center">
  
  <h1> Device Base </h1>


</div>

<br/>

Projekt wykorzystuje technologie .NET Web API, Entity Framework (C#) i serwer bazodanowy SQL Server (T-SQL).
Podejście tworzenia projektu: Code First.

### Opis Systemu

System jest globalny oparty o SQL Server.
System służy do przechowywania informacji o urządzeniach IoT typu kontroler pasków led / inteligentna żarówka / inteligentna brama.

Każde fizyczne urządzenie ma swój typ, typ zawiera podstawowe informacje wspólne dla wszystkich urządzeń np maksymalna liczba użytkowników która może się podłączyć do urządzenia. Typ jest tak jak model urządzenia, w bazie nie przechowywujemy wszytkich informacji o modelu urzadzenia raczej zakładamy że tutaj administrator może wprowadzic nowy model z innego systemu np. stricte zwiazanego już z zespołem tworzącym nowe modele urządzeń.

System zakłada że informacje o fizycznym urządzeniu są zawsze w bazie danych tzn. są dodawane przy wypuszczaniu urzadzenia z fabryki i dodawane do bazy danych przez administratora. Następnie użytkownik może się do nich połaczyć i edytować wybrane wartośći urządzenia. (Proces podłączania do bazy opisany niżej)

System zawiera 3 głównych aktorów:
- Niezautoryzowany użytkonik - jedyne akcje jakie może podjąć to rejestracja bądż logowanie
- Zautoryzowanego użytkownika - posiada ograniczone uprawnienia w systemie - nie może wysyłać zapytań na niektóre endpointy, np. nie może dodawać urządzeń - ale może zarządzać swoimi urządzeniami
- Administratora - może wysyłać zapytanie na każdy endpoint - administrator dba między innymi o dodawanie nowych typów urządzeń do bazy oraz fizycznych urządzeń

Endpointy możemy pogrupować:
- Endpointy Autoryzacji - mają na celu logowanie / rejestrowanie użytkownika bądź odświeżanie tokenów użytkownika (autoryzacja oparta o JWT i OAuth 2)
- Endpointy Urządzeń - zawierają podstawowe operacje CRUD oraz metody pozwalające na łączenie się z urządzeniami. Metody te są wywoływane przez aplikacje po połączeniu się użytkownika z urządzeniem
- Endpointy Typów Urządzeń - zawieraję podstawowe operacje CRUD dla modeli urządzeń
- Endpointy Użytkownika - pozwalają użytkownikowi na modyfikowanie niektórych swoich ustawień oraz pobieranie ich 

#### Baza danych 
Uwaga początkowa każdy model w naszej bazie (oprócz użytkownika) używa ``` BaseModel ```.

 ```cs 
public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
} 
```

Baza danych została stworzona z podejściem code-first. Rozpoczynaliśmy więc od stworzenia modelu, nastepnie logiki działania endpointu, a na końcu tworzyliśmy migracje.
```cs
  public class Device : BaseModel
{
    public string DeviceName { get; set; }
    public string DevicePlacing { get; set; }
    public string Description { get; set; }

    public string OwnerId { get; set; }
    public string DeviceSecret { get; set; }

    public bool NewConnectionsPermitted { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();

    public int DeviceTypeId { get; set; }
    public DeviceType DeviceType { get; set; }

    public string MqttUrl { get; set; }
    public string SerialNumber { get; set; }

    public DateTime Produced { get; set; }
}
```
Powyższy przykładowy model był używany w serwisie np. do pobierania urządzeń użytkownika. 
```cs
 public async Task<IEnumerable<Device>> GetUserItemsAsync(string guid)
    {
        var user = await db.AppUsers
            .Include(x => x.Devices)
            .SingleOrDefaultAsync(x => x.Id == guid);

        if (user == null)
            return null;

        var devices = user.Devices.Select(x => { x.Users = null; return x; });

        return devices;
    }

```


<div> 
    <img src="https://github.com/psp515/DeviceBase/blob/main/docs/database.png" />
</div>



Baza danych zawiera wiele tabel; duża część z nich jest generowana dla potrzeb wewnętrznych Identity Component, który wykorzystywany jest do autoryzacji i autentykacji.
Oprócz tego baza zawiera tabele z urządzeniami i ich typami oraz tabele przejścia pomiędzy urządzeniami oraz użytkownikami.

##### Tabele AspNetRoles, AspNetRolesClaims i AspNetUserRoles 
Zawierają informacje o dostępnych rolach w systemie i jakie role ma dany użytkownik - rola zapewnia dostęp do konkretnych akcji w bazie. Poniżej przykład (```  .RequireAuthorization ```). 

```cs
        /* Application Admin Policy */
        application.MapPost("/api/devices", CreateItem)
            .WithName("Create Device")
            .Accepts<DeviceCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        /* Application User Policy */
        application.MapGet("/api/devices/user", GetUserItemsAsync)
            .WithName("Get User Devices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);
```

##### Tabela AspNetUserTokens i AspNetUserLogins 
Istotne przy logowaniu i odświeżaniu tokenów aby użytkownik mógł dalej korzystać z aplikacji. Poniżej metoda odpowiadająca za zalogowanie użytkownika.
```cs
  public async Task<ServiceResult> Login(string email, string password)
    {
        var user = await db.AppUsers.SingleOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return new ServiceResult(false, "User not found.");

        bool isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
            return new ServiceResult(false, "Invalid password or email.");

        await ChcekRoles();

        var result = await CreateTokens(user);

        return result;
    }
```

##### Tabela AspNetUsers
Zawiera podstawowe informacje o użytkowniku i te potrzebne do zautoryzowania. Tabela ta zawiera podstawoe pola ```cs IdentityUser``` oraz jest rozbudowana o pola potrzebne dla nas.

Ciekawym aspektem są indeksy które generują się same.
```cs
public class User : IdentityUser
{
    public List<Device> Devices { get; set; } = new List<Device>();

    public AppModeEnum AppMode { get; set; }
    public LanguageEnum Language { get; set; }
    public string ImageUrl { get; set; }
    public bool Sounds { get; set; }
    public bool PushNotifications { get; set; }
    public bool Localization { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}
```
```sql
create table AspNetUsers
(
    Id                   nvarchar(450) not null
        constraint PK_AspNetUsers
            primary key,
    AppMode              int           not null,
    Language             int           not null,
    ImageUrl             nvarchar(max),
    Sounds               bit           not null,
    PushNotifications    bit           not null,
    Localization         bit           not null,
    Edited               datetime2     not null,
    Created              datetime2     not null,
    UserName             nvarchar(256),
    NormalizedUserName   nvarchar(256),
    Email                nvarchar(256),
    NormalizedEmail      nvarchar(256),
    EmailConfirmed       bit           not null,
    PasswordHash         nvarchar(max),
    SecurityStamp        nvarchar(max),
    ConcurrencyStamp     nvarchar(max),
    PhoneNumber          nvarchar(max),
    PhoneNumberConfirmed bit           not null,
    TwoFactorEnabled     bit           not null,
    LockoutEnd           datetimeoffset,
    LockoutEnabled       bit           not null,
    AccessFailedCount    int           not null
)
go

create index EmailIndex
    on AspNetUsers (NormalizedEmail)
go

create unique index UserNameIndex
    on AspNetUsers (NormalizedUserName)
    where [NormalizedUserName] IS NOT NULL
go

```

##### DeviceUser i Device

DeviceUser to tabela łącząca użytkowników oraz urządzenia.

Device to tabela zawierające fizyczne urządzenia które możemy załączać do naszych kont w aplikacji. Łączenie do urządzenia w przykładach poniżej. Jest to Tabela posiadająca najwięcej endpointów i różnych metod które na niej operują. 
```cs
 /* Admin Policies */

        application.MapGet("/api/devices", GetAllAsync)
            .WithName("Get Devices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapGet("/api/devices/{id:int}", GetItemAsync)
            .WithName("Get Device")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPost("/api/devices", CreateItem)
            .WithName("Create Device")
            .Accepts<DeviceCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        /* Application User Policies */

        application.MapGet("/api/devices/user", GetUserItemsAsync)
            .WithName("Get User Devices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapGet("/api/devices/{id:int}/users", GetDeviceUsers)
            .WithName("Get Users Connected to Device")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPut("/api/devices/{id:int}", UpdateItem)
            .WithName("Update Device")
            .Accepts<DeviceUpdateDTO>("application/json")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/connect", ConnectDevice)
            .WithName("Connect Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/disconnect", DisconnectDevice)
            .WithName("Disconnect Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/connectowner", ConnectOwner)
            .WithName("Connect Owner to Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/disconnectowner", DisconnectOwner)
            .WithName("Disconnect Owner from Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/ownerdisconnectuser", OwnerDisconnectsUser)
            .WithName("Disconnect User from Device By Owner")
            .Accepts<DisconnectUserDTO>("application/json")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}", ChangeDevicePolicy)
            .WithName("Change Device Connection Policy")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);
```

```sql
create table Devices
(
    Id                      int identity
        constraint PK_Devices
            primary key,
    DeviceName              nvarchar(max),
    DevicePlacing           nvarchar(max),
    Description             nvarchar(max),
    OwnerId                 nvarchar(450)
        constraint FK_Owner
            references AspNetUsers,
    DeviceSecret            nvarchar(max),
    NewConnectionsPermitted bit       not null,
    DeviceTypeId            int       not null
        constraint FK_Devices_DeviceTypes_DeviceTypeId
            references DeviceTypes
            on delete cascade,
    MqttUrl                 nvarchar(max),
    SerialNumber            nvarchar(max),
    Produced                datetime2 not null,
    Edited                  datetime2 not null,
    Created                 datetime2 not null
)
go

create index IX_Devices_DeviceTypeId
    on Devices (DeviceTypeId)
go

create table DeviceUser
(
    DevicesId int           not null
        constraint FK_DeviceUser_Devices_DevicesId
            references Devices
            on delete cascade,
    UsersId   nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_DeviceUser_AspNetUsers_UsersId
            references AspNetUsers
            on delete cascade,
    constraint PK_DeviceUser
        primary key (DevicesId, UsersId)
)
go

create index IX_DeviceUser_UsersId
    on DeviceUser (UsersId)
go

```

##### Tabela DeviceType

Jest to tabela zawierająca podstawowe informacje wspólne dla modeli urządzeń np. mamy kontroler LED SPE-61 i jego podstawową własnością jest to, żę maksymalnie 5 użytkowników może wysyłać do niego informacje. Tak jak wspominaliśmy wcześniej, zakładamy, że w systemie nie potrzebujemy wszystkich informacji i po prostu te wymagane przez nas są zaimportowane do naszego REST API.
```cs
public class DeviceType : BaseModel
{
    public string DefaultName { get; set; }
    public string EndpointsJson { get; set; }
    public int MaximalNumberOfUsers { get; set; }

    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
```

```sql
create table DeviceTypes
(
    Id                   int identity
        constraint PK_DeviceTypes
            primary key,
    DefaultName          nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    EndpointsJson        nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    MaximalNumberOfUsers int       not null,
    Edited               datetime2 not null,
    Created              datetime2 not null
)
go
```

#### Przykładowy przypadek użycia 1 - Łączenie się do urządzenia przez właściciela (który chwilę temu kupił produkt)

Endpoint: 
```cs
private async Task<IResult> ConnectOwner(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             [FromBody] OwnerConnectionDTO dto,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.ConnectOwner(id, guid, dto.DeviceSecret);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }
```

Serwis:

```cs
 public async Task<ServiceResult> ConnectOwner(int deviceId, string userId, string secret)
    {
        var device = await db.Devices
           .Include(x => x.DeviceType)
           .Include(x => x.Users)
           .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (!string.IsNullOrEmpty(device.OwnerId))
            return new ServiceResult(false, "Device has owner.");

        if (device.DeviceSecret != secret)
            return new ServiceResult(false, "Invalid secret.");

        var user = await db.Users
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == userId);

        device.OwnerId = userId;
        user.Devices.Add(device);

        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }
```

1. Z telefonem, na który pobraliśmy aplikację jesteśmy w zasiegu bluetooth urządenia.
2. Aplikacja wykrywa urządzenie i proponuje połaczenie się z nim jako gość / jako jedyny właściciel.
3. Użytkownik wybiera opcje 2.
4. Aplikacja pyta o sekretny klucz urządzenia (klucz dostarczony z urządzeniem w pudełku / hasło gdzieś na urządzeniu jak np. w przypadku routerów informacje takie jak początkowe hasło są dodawane na spodzie).
5. Użytkownik podaje hasło i oczekuje na połączenie.
6a. Udane połączenie: można korzystać z urządzenia.
6b. Nieudane połączenie: urządzenie posiada własciciela - jeżeli kupiliśmy nowy produkt, to powinniśmy się skontakować z supportem.

Widzimy iż generowane zapytania korzystają z podzapytań ale tak samo z JOIN. Zapytania można by zopytmalizować np. napisać bez używania podzapytań.
```sql
SELECT [t].[Id], [t].[Created], [t].[Description], [t].[DeviceName], [t].[DevicePlacing], [t].[DeviceSecret], [t].[DeviceTypeId], [t].[Edited], [t].[MqttUrl], [t].[NewConnectionsPermitted], [t].[OwnerId], [t].[Produced], [t].[SerialNumber], [t].[Id0], [t].[Created0], [t].[DefaultName], [t].[Edited0], [t].[EndpointsJson], [t].[MaximalNumberOfUsers], [t0].[DevicesId], [t0].[UsersId], [t0].[Id], [t0].[AccessFailedCount], [t0].[AppMode], [t0].[ConcurrencyStamp], [t0].[Created], [t0].[Edited], [t0].[Email], [t0].[EmailConfirmed], [t0].[ImageUrl], [t0].[Language], [t0].[Localization], [t0].[LockoutEnabled], [t0].[LockoutEnd], [t0].[NormalizedEmail], [t0].[NormalizedUserName], [t0].[PasswordHash], [t0].[PhoneNumber], [t0].[PhoneNumberConfirmed], [t0].[PushNotifications], [t0].[SecurityStamp], [t0].[Sounds], [t0].[TwoFactorEnabled], [t0].[UserName]
      FROM (
          SELECT TOP(1) [d].[Id], [d].[Created], [d].[Description], [d].[DeviceName], [d].[DevicePlacing], [d].[DeviceSecret], [d].[DeviceTypeId], [d].[Edited], [d].[MqttUrl], [d].[NewConnectionsPermitted], [d].[OwnerId], [d].[Produced], [d].[SerialNumber], [d0].[Id] AS [Id0], [d0].[Created] AS [Created0], [d0].[DefaultName], [d0].[Edited] AS [Edited0], [d0].[EndpointsJson], [d0].[MaximalNumberOfUsers]
          FROM [Devices] AS [d]
          INNER JOIN [DeviceTypes] AS [d0] ON [d].[DeviceTypeId] = [d0].[Id]
          WHERE [d].[Id] = @__deviceId_0
      ) AS [t]
      LEFT JOIN (
          SELECT [d1].[DevicesId], [d1].[UsersId], [a].[Id], [a].[AccessFailedCount], [a].[AppMode], [a].[ConcurrencyStamp], [a].[Created], [a].[Edited], [a].[Email], [a].[EmailConfirmed], [a].[ImageUrl], [a].[Language], [a].[Localization], [a].[LockoutEnabled], [a].[LockoutEnd], [a].[NormalizedEmail], [a].[NormalizedUserName], [a].[PasswordHash], [a].[PhoneNumber], [a].[PhoneNumberConfirmed], [a].[PushNotifications], [a].[SecurityStamp], [a].[Sounds], [a].[TwoFactorEnabled], [a].[UserName]
          FROM [DeviceUser] AS [d1]
          INNER JOIN [AspNetUsers] AS [a] ON [d1].[UsersId] = [a].[Id]
      ) AS [t0] ON [t].[Id] = [t0].[DevicesId]
      ORDER BY [t].[Id], [t].[Id0], [t0].[DevicesId], [t0].[UsersId]


 SELECT [t].[Id], [t].[AccessFailedCount], [t].[AppMode], [t].[ConcurrencyStamp], [t].[Created], [t].[Edited], [t].[Email], [t].[EmailConfirmed], [t].[ImageUrl], [t].[Language], [t].[Localization], [t].[LockoutEnabled], [t].[LockoutEnd], [t].[NormalizedEmail], [t].[NormalizedUserName], [t].[PasswordHash], [t].[PhoneNumber], [t].[PhoneNumberConfirmed], [t].[PushNotifications], [t].[SecurityStamp], [t].[Sounds], [t].[TwoFactorEnabled], [t].[UserName], [t0].[DevicesId], [t0].[UsersId], [t0].[Id], [t0].[Created], [t0].[Description], [t0].[DeviceName], [t0].[DevicePlacing], [t0].[DeviceSecret], [t0].[DeviceTypeId], [t0].[Edited], [t0].[MqttUrl], [t0].[NewConnectionsPermitted], [t0].[OwnerId], [t0].[Produced], [t0].[SerialNumber]
      FROM (
          SELECT TOP(1) [a].[Id], [a].[AccessFailedCount], [a].[AppMode], [a].[ConcurrencyStamp], [a].[Created], [a].[Edited], [a].[Email], [a].[EmailConfirmed], [a].[ImageUrl], [a].[Language], [a].[Localization], [a].[LockoutEnabled], [a].[LockoutEnd], [a].[NormalizedEmail], [a].[NormalizedUserName], [a].[PasswordHash], [a].[PhoneNumber], [a].[PhoneNumberConfirmed], [a].[PushNotifications], [a].[SecurityStamp], [a].[Sounds], [a].[TwoFactorEnabled], [a].[UserName]
          FROM [AspNetUsers] AS [a]
          WHERE [a].[Id] = @__userId_0
      ) AS [t]
      LEFT JOIN (
          SELECT [d].[DevicesId], [d].[UsersId], [d0].[Id], [d0].[Created], [d0].[Description], [d0].[DeviceName], [d0].[DevicePlacing], [d0].[DeviceSecret], [d0].[DeviceTypeId], [d0].[Edited], [d0].[MqttUrl], [d0].[NewConnectionsPermitted], [d0].[OwnerId], [d0].[Produced], [d0].[SerialNumber]
          FROM [DeviceUser] AS [d]
          INNER JOIN [Devices] AS [d0] ON [d].[DevicesId] = [d0].[Id]
      ) AS [t0] ON [t].[Id] = [t0].[UsersId]
      ORDER BY [t].[Id], [t0].[DevicesId], [t0].[UsersId]


 SET NOCOUNT ON;
      INSERT INTO [DeviceUser] ([DevicesId], [UsersId])
      VALUES (@p0, @p1);
      UPDATE [Devices] SET [OwnerId] = @p2
      OUTPUT 1
      WHERE [Id] = @p3;

```

#### Przykładowy przypadek użycia 2 - Łączenie się do urządzenia przez kolegę

Endpoint: 

```cs
 private async Task<IResult> ConnectDevice(IDeviceService service,
                                              IConfiguration configuration,
                                              [FromHeader(Name = "Authorization")] string bearerToken,
                                              int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.ConnectDevice(id, guid);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }
```

Serwis:

```cs
public async Task<ServiceResult> ConnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.DeviceType)
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (device.DeviceType.MaximalNumberOfUsers < device.Users.Count)
            return new ServiceResult(false, "Cannot connect new user.");

        if (string.IsNullOrEmpty(device.OwnerId))
            return new ServiceResult(false, "Device doesn't have a owner.");

        if (device.Users.Any(x => x.Id == userId))
            return new ServiceResult(false, "User already connected.");

        if (!device.NewConnectionsPermitted)
            return new ServiceResult(false, "New connections not perrmited for this device.");

        var user = await db.Users
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == userId);

        user.Devices.Add(device);
        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }
```

1. Z telefonem, na który pobraliśmy aplikację jesteśmy w zasiegu bluetooth urządenia.
2. Aplikacja wykrywa urządzenie i propouje połaczenie się z nim jako gość / jako jedyny właściciel.
3. Użytkownik wybiera opcje 1.
4. Aplikacja łączy nas z urządzeniem (nie pyta wlaściciela o zgode).
5a. Udane połączenie: można korzystać z urządzenia.
5b. Nieudane połączenie: właściciel urządzenia zablokował możliwość łączenia do urządzenia.


Dokładnie to samo, widzimy iż generowane zapytania korzystają z podzapytań ale tak samo z JOIN. Zapytania można by zopytmalizować np. napisać bez używania podzapytań. Zapytania dalej bardzo rozbudowane w stosunku do kodu C#.

```sql
SELECT [t].[Id], [t].[Created], [t].[Description], [t].[DeviceName], [t].[DevicePlacing], [t].[DeviceSecret], [t].[DeviceTypeId], [t].[Edited], [t].[MqttUrl], [t].[NewConnectionsPermitted], [t].[OwnerId], [t].[Produced], [t].[SerialNumber], [t].[Id0], [t].[Created0], [t].[DefaultName], [t].[Edited0], [t].[EndpointsJson], [t].[MaximalNumberOfUsers], [t0].[DevicesId], [t0].[UsersId], [t0].[Id], [t0].[AccessFailedCount], [t0].[AppMode], [t0].[ConcurrencyStamp], [t0].[Created], [t0].[Edited], [t0].[Email], [t0].[EmailConfirmed], [t0].[ImageUrl], [t0].[Language], [t0].[Localization], [t0].[LockoutEnabled], [t0].[LockoutEnd], [t0].[NormalizedEmail], [t0].[NormalizedUserName], [t0].[PasswordHash], [t0].[PhoneNumber], [t0].[PhoneNumberConfirmed], [t0].[PushNotifications], [t0].[SecurityStamp], [t0].[Sounds], [t0].[TwoFactorEnabled], [t0].[UserName]
      FROM (
          SELECT TOP(1) [d].[Id], [d].[Created], [d].[Description], [d].[DeviceName], [d].[DevicePlacing], [d].[DeviceSecret], [d].[DeviceTypeId], [d].[Edited], [d].[MqttUrl], [d].[NewConnectionsPermitted], [d].[OwnerId], [d].[Produced], [d].[SerialNumber], [d0].[Id] AS [Id0], [d0].[Created] AS [Created0], [d0].[DefaultName], [d0].[Edited] AS [Edited0], [d0].[EndpointsJson], [d0].[MaximalNumberOfUsers]
          FROM [Devices] AS [d]
          INNER JOIN [DeviceTypes] AS [d0] ON [d].[DeviceTypeId] = [d0].[Id]
          WHERE [d].[Id] = @__deviceId_0
      ) AS [t]
      LEFT JOIN (
          SELECT [d1].[DevicesId], [d1].[UsersId], [a].[Id], [a].[AccessFailedCount], [a].[AppMode], [a].[ConcurrencyStamp], [a].[Created], [a].[Edited], [a].[Email], [a].[EmailConfirmed], [a].[ImageUrl], [a].[Language], [a].[Localization], [a].[LockoutEnabled], [a].[LockoutEnd], [a].[NormalizedEmail], [a].[NormalizedUserName], [a].[PasswordHash], [a].[PhoneNumber], [a].[PhoneNumberConfirmed], [a].[PushNotifications], [a].[SecurityStamp], [a].[Sounds], [a].[TwoFactorEnabled], [a].[UserName]
          FROM [DeviceUser] AS [d1]
          INNER JOIN [AspNetUsers] AS [a] ON [d1].[UsersId] = [a].[Id]
      ) AS [t0] ON [t].[Id] = [t0].[DevicesId]
      ORDER BY [t].[Id], [t].[Id0], [t0].[DevicesId], [t0].[UsersId]


SELECT [t].[Id], [t].[AccessFailedCount], [t].[AppMode], [t].[ConcurrencyStamp], [t].[Created], [t].[Edited], [t].[Email], [t].[EmailConfirmed], [t].[ImageUrl], [t].[Language], [t].[Localization], [t].[LockoutEnabled], [t].[LockoutEnd], [t].[NormalizedEmail], [t].[NormalizedUserName], [t].[PasswordHash], [t].[PhoneNumber], [t].[PhoneNumberConfirmed], [t].[PushNotifications], [t].[SecurityStamp], [t].[Sounds], [t].[TwoFactorEnabled], [t].[UserName], [t0].[DevicesId], [t0].[UsersId], [t0].[Id], [t0].[Created], [t0].[Description], [t0].[DeviceName], [t0].[DevicePlacing], [t0].[DeviceSecret], [t0].[DeviceTypeId], [t0].[Edited], [t0].[MqttUrl], [t0].[NewConnectionsPermitted], [t0].[OwnerId], [t0].[Produced], [t0].[SerialNumber]
      FROM (
          SELECT TOP(1) [a].[Id], [a].[AccessFailedCount], [a].[AppMode], [a].[ConcurrencyStamp], [a].[Created], [a].[Edited], [a].[Email], [a].[EmailConfirmed], [a].[ImageUrl], [a].[Language], [a].[Localization], [a].[LockoutEnabled], [a].[LockoutEnd], [a].[NormalizedEmail], [a].[NormalizedUserName], [a].[PasswordHash], [a].[PhoneNumber], [a].[PhoneNumberConfirmed], [a].[PushNotifications], [a].[SecurityStamp], [a].[Sounds], [a].[TwoFactorEnabled], [a].[UserName]
          FROM [AspNetUsers] AS [a]
          WHERE [a].[Id] = @__userId_0
      ) AS [t]
      LEFT JOIN (
          SELECT [d].[DevicesId], [d].[UsersId], [d0].[Id], [d0].[Created], [d0].[Description], [d0].[DeviceName], [d0].[DevicePlacing], [d0].[DeviceSecret], [d0].[DeviceTypeId], [d0].[Edited], [d0].[MqttUrl], [d0].[NewConnectionsPermitted], [d0].[OwnerId], [d0].[Produced], [d0].[SerialNumber]
          FROM [DeviceUser] AS [d]
          INNER JOIN [Devices] AS [d0] ON [d].[DevicesId] = [d0].[Id]
      ) AS [t0] ON [t].[Id] = [t0].[UsersId]
      ORDER BY [t].[Id], [t0].[DevicesId], [t0].[UsersId]


SET IMPLICIT_TRANSACTIONS OFF;
      SET NOCOUNT ON;
      INSERT INTO [DeviceUser] ([DevicesId], [UsersId])
      VALUES (@p0, @p1);

```

#### Przykładowy przypadek użycia 3 - Rozłączenie właściciela

Przydatne, gdy sprzedajemy urządzenie.

Endpoint: 

```cs
 private async Task<IResult> DisconnectOwner(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.DisconnectOwner(id, guid);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }
```

Serwis:

```cs
 public async Task<ServiceResult> DisconnectOwner(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (device.OwnerId != userId)
            return new ServiceResult(false, "User is not owner.");

        var user = await db.Users.FirstAsync(x => x.Id == userId);

        device.Users.Clear();
        device.OwnerId = "";

        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }
```

1. W aplikacji klikamy rozłącz z urządzeniem.
2. Aplikacja rozłącza właściciela oraz rozłącza wszystkich użytkowników, ponieważ urządzenie nie ma właściciela. Przed ponownym skorzystaniem z urządzenia konieczne jest dodanie nowego właściciela.

Kody sql dalej duze i rozbudowane (nie zostały wklejone bo to jest praktycznie powtórzenie 3 raz czegoś bardzo podobnego). Jedyną różnica sa tutaj linie odpowiadajace za usuwanie połączeń w bazie danych, to również można by zoptymalizować usuwając po prostu wszystkie pole gdzie mamy odpowiedni id urządzenia.

```sql

      SET NOCOUNT ON;
      DELETE FROM [DeviceUser]
      OUTPUT 1
      WHERE [DevicesId] = @p0 AND [UsersId] = @p1;
      DELETE FROM [DeviceUser]
      OUTPUT 1
      WHERE [DevicesId] = @p2 AND [UsersId] = @p3;
      UPDATE [Devices] SET [OwnerId] = @p4
      OUTPUT 1
      WHERE [Id] = @p5;
```
