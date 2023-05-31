<div align="center">
  
  <h1> Device Base </h1>

<br/>
  
  Projekt wykorzystuje technologie `**.NET WebAPI**`, **Entity Framework** i serwer bazodanowy **MySQL**, pisany za pomocą języka **C#** 
</div>

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

##### DeviceType

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

#### Przykładowy przypadek użycia - Łączenie się do urządzenia przez właściciela (który chwilę temu kupił produkt)

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
