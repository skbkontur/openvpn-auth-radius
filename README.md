Introduction
============
[![Build status](https://img.shields.io/appveyor/ci/chipitsine/openvpn-auth-radius.svg?label=build windows)](https://ci.appveyor.com/project/chipitsine/openvpn-auth-radius) 

if you are familiar with C#, you might want to wrap server side OpenVPN authentication with openvpn-auth-radius

Usage
------

add the following line to server.conf:

```
auth-user-pass-verify 'mono /etc/openvpn/auth.exe' via-env
```

example auth.exe.config:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="MyConfig" type="auth.Config,  auth" />
  </configSections>
  <MyConfig NAS_IDENTIFIER="OpenVpn">
    <servers>
      <server name="radius1.example" authport="1812" wait="1" retries="3" sharedsecret="secret1" />
      <server name="radius2.example" authport="1812" wait="1" retries="2" sharedsecret="secret2" />
    </servers>
  </MyConfig>
</configuration>
```
