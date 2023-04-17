<div align="center">
  
  <h1> Device Base </h1>
  <p> API example for iot devices </p>
  
  <div>
    <a href="">
      <img src="https://img.shields.io/github/last-commit/psp515/DeviceBase" alt="last update" />
    </a>
    <a href="https://github.com/psp515/DeviceBase/network/members">
      <img src="https://img.shields.io/github/forks/psp515/DeviceBase" alt="forks" />
    </a>
    <a href="https://github.com/psp515/DeviceBase/stargazers">
      <img src="https://img.shields.io/github/stars/psp515/DeviceBase" alt="stars" />
    </a>
    <a href="https://github.com/psp515/DeviceBase/issues/">
      <img src="https://img.shields.io/github/issues/psp515/DeviceBase" alt="open issues" />
    </a>
    <a href="https://github.com/psp515/DeviceBase/blob/master/LICENSE">
      <img src="https://img.shields.io/github/license/psp515/DeviceBase" alt="license" />
    </a>
  </div>
</div>  

<br/>

### About The Project

Device base is small databes for variable devices. Users are able to create accounts and connect to devices.
Each device have maximal number of user that are able to connect to device. Also user is able to store his appliaction settings.

Provided Endpoints:
- auth - endpoint for authorization and authentication of user - contains base endpoints for logging and registering user - created with use of IdentityUser
- device - endpoint for managing physical devices - provides methods for 2 roles - user is able to update device and connect / discounnet to device 
- user [inprogress] - endpoint for basic user request as get information about user some endpoints for admin user might be added
- devicetype [inprogress] - endpoint describes base properties of device for admin users only 
- devicecharacteristic [under consiederation] - endpoint might describe posible use cases of the device 
- deviceschedules [under consiederation] - endpoint might describe device working schedules provided by users.  

Database is creared with code first approach.

### Built With

<div> 
  <a>
    <img src="https://img.shields.io/badge/-CSharp-2E8B57?logo=csharp" />
  </a>
  <a>
    <img src="https://img.shields.io/badge/.NET_Web_API-0089D6?style=for-the-badge&logo=dotnet&logoColor=white&style=flat" />
  </a>
  <a>
    <img src="https://img.shields.io/badge/Entity_Framework-0089D6?style=for-the-badge&logo=dotnet&logoColor=white&style=flat" />
  </a>
  <a>
    <img src="https://img.shields.io/badge/TSQL-239120?style=for-the-badge&logo=microsoft-sql-server&logoColor=white&style=flat" />
  </a>
</div>

### Roadmap 

Area| Finished | Approved
:-: | :-: | :-: 
Setup | ✅  | ✅ 
Auth endpoint | ✅ | 🚧 
Device endpoint | ✅ | 🚧 
Device type endpoint | 🚧 | ❌ 
User endpoint | 🚧 | ❌ 
Enhancing endpoints | 🚧 | ❌ 
Database Schema | ❌  | ❌ 



### License

Distributed under the MIT License. See `LICENSE.txt` for more information.

# Contact

- Wiktor Gut, wiktorgut@student.agh.edu.pl 
- Piotr Olszak, olsza@student.agh.edu.pl
- Łukasz Kolber, lukaszkolber@studnt.agh.edu.pl
