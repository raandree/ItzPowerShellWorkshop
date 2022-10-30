# Itz PowerShell Workshop

## Recommended PowerShell Modules

- [Sampler: Module template with build pipeline and examples, including DSC elements)](https://github.com/gaelcolas/Sampler)
- [AutomatedLab - Lab automation from users for users](https://automatedlab.org)
- [NTFSSecurity: Managing permissions with PowerShell](https://github.com/raandree/NTFSSecurity)
- [ConvertTo-Expression: Serializes an object to a PowerShell expression](https://github.com/iRon7/ConvertTo-Expression)
- [Join-Object: Combines two objects lists based on a related property between them](https://github.com/iRon7/Join-Object)
- [ThreadJob: A PowerShell module for running concurrent jobs based on threads rather than processes](https://github.com/PowerShell/ThreadJob)
- [AutomatedLab.Common](https://github.com/AutomatedLab/AutomatedLab.Common)
- [PSPKI](https://github.com/PKISolutions/PSPKI)

## Recommended Resources

- [PowerShell Explained](https://powershellexplained.com)
- [From a one-liner to an advanced function and module](https://github.com/raandree/PowerShellTraining)
- [PowerShell.one](https://powershell.one/)

## Visual Studio Code

**Keep in mind the ```F1``` shortcut which give you access to literally every command.**  

### Extensions
> Note: You can automatically install extensions into VSCode by calling ```code --install-extension <path>```
- [PowerShell](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- [GitLense](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens)
- [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)
- [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.ode-spell-checker)
- [YAML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml)
- [vscode-icons](https://marketplace.visualstudio.com/items?itemName=vscode-icons-team.vscode-icons)
- [Bookmarks](https://marketplace.visualstudio.com/items?itemName=alefragnani.Bookmarks)
- [GitHub Copilot](https://marketplace.visualstudio.com/items?itemName=GitHub.copilot)

### Config
- Each VSCode project can have it own settings when storing them in the file ```settings.json``` which must be n the ```.vscode``` folder. These settings may contain code style settings and settings for extensions.

### Debugging

- You can create yourself debug configs for each project. This is stored in the ```launch.json``` file in the ``.vscode``` folder.
- Breakpoints can have added expressions or hit count, which makes debugging of loops much more comfortable.
- The watch pane is most effective way to get an overview of you variables and their content.
- Use the call stack that shows you the path to the current breakpoint in your code. You can easily navigate ack to the part of the code that called the code you are currently having the breakpoint in.
- Remote debugging works in the ISE by opening a script with the command ```psEdit``` in the remote machine ithin a interactive session. If the workflow to start the script remotely is more complex, you can use the mdlet ```Wait-Debugger``` to halt the process and attache to it with ```Enter-PSHostProcess```. You need to now the process ID which is reflected by the variable ```$PID```.
Git Integration
A simple Git implementation is built into VSCode by default. The extension [GitLense](https://marketplace.isualstudio.com/items?itemName=eamodio.gitlens) additionally gives you a graphical file and commit history.

> You can find more information about [debugging](https://code.visualstudio.com/Docs/editor/debugging).

### Offline install of vscode extensions

This code was taken from [DscWorkshop/Lab/20 Lab Customizations.ps1](https://github.com/dsccommunity/DscWorkshop/blob/fdd283fa9a0cee53f1a749352969b387e752a0a5/Lab/20%20Lab%20Customizations.ps1#L166)

```powershell
Copy-LabFileItem -Path $labSources\SoftwarePackages\VSCodeExtensions -ComputerName $devOpsServer
Invoke-LabCommand -ActivityName 'Install VSCode Extensions' -ComputerName $devOpsServer -ScriptBlock {
    dir -Path C:\VSCodeExtensions | ForEach-Object {
        code --install-extension $_.FullName 2>$null #suppressing errors
    }
} -NoDisplay
```

&nbsp;

> Note: You can download any VSCode extension from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell) like for example.

&nbsp;

## Code Samples

- ### ThreadJob are running within the same thread and are more efficient

    ```powershell
    #Install-Module ThreadJob -Force
    $sb = { Get-Random; Start-Sleep -Milliseconds 2000 }

    1..100 | ForEach-Object {
        #Start-Job -ScriptBlock $sb
        Start-ThreadJob -ScriptBlock $sb
    } | Receive-Job -AutoRemoveJob -Wait

    $r.Count
    ```

    &nbsp;

- ### ForEach-Object and foreach statement

    When used with the pipeline, `foreach` is an alias to `ForEach-Object`. When used in the this syntax, PowerShell uses the `foreach` statement: `foreach (<iterator> in <list>)`.

    The `foreach` statement supports `continue` and `break`, whereas `ForEach-Object` only supports `break`. Additionally the latter one is much faster.

    ```powershell
    $chars = 'a', 'b', 'c'
    $numbers = 1, 2, 3

    foreach ($char in $chars) {
        foreach ($number in $numbers) {
            "$char - $number"
        }
    }

    $chars | ForEach-Object {
        $temp = $_
        if ($_ -eq 'b') { continue }
        $numbers | foreach {
            "$temp - $_"
        }
    }
    <#
            a - 1
            a - 2
            ...
            c - 3
    #>
    ```

    &nbsp;

- ### Strings, expandable, non-expandable and the escape character

    The backtick is (`` ` ``) is the escape character in PowerShell which does that: "In computing and telecommunication, an escape character is a character that **invokes an alternative interpretation on the following characters** in a character sequence. An escape character is a particular case of metacharacters. Generally, the judgement of whether something is an escape character or not depends on the context." ([Escape character - Wikipedia](https://en.wikipedia.org/wiki/Escape_character)).

    &nbsp;

    ```powershell
    $p = Get-Process -Id $PID

    "My current process consumes $p.WS bytes"
    "My current process consumes $($p.WS / 1MB) MB"
    "My current process consumes $([math]::Round($p.WS / 1MB, 2)) MB"

    'My current process consumes {0} MB' -f ($p.WS / 1MB)
    'My current process consumes {0} MB' -f [math]::Round(($p.WS / 1MB), 2)
    'My current process consumes {0:N2} MB' -f ($p.WS / 1MB)

    1..100 | ForEach-Object {
        "TestUser {0:D4}" -f $_
    }

    return
    "The value of `$a is '$a'"
    'The value of $(a) is $a'

    "Hello `n World in `t 2022"
    ```

    &nbsp;

- ### Error Handling

    The use of `ErrorAction Stop` is rarely used, mainly when a cmdlet call is made in a try-block that should have the possibility of failing

    With `$_` inside the catch block you get access to the caught exception.

    ```powershell
    dir -Recurse -Path C:\Windows -ErrorVariable myError

    try {
        dir y: -ErrorAction Stop
    }
    catch {
        Write-Warning $_.Exception.Message
    }
    ```

    &nbsp;

- ### PowerShell Classes implementing an interface

    This class implements the [`IEnumerable`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=net-7.0) interface.

    ```powershell
    class Test1 : System.Collections.IEnumerable, 
    {
        hidden [object[]]$list
        
        [System.Collections.IEnumerator] GetEnumerator() {
            return $this.list.GetEnumerator()
        }
        
        Test1([object[]]$values)
        {
            $this.list = $values
        }
    }

    $o = [Test1]::New((1, 2, 3))
    $o.GetEnumerator()
    ```

- ### Regular Expressions

  Regular expressions offer the most effective way to find patterns in text, extract and / or replace parts of text.

  Some helpful references:
  - [Mike Kanakos on Twitter: "Ever wish you could        learn Regex in a simple, straightforward tutorial with one of the best speakers in the PowerShell community? Check out this awesome intro to Regex with @MrThomasRayner #PowerShell #Automation](https://t.co/z3gwsYTpDt)
  - [regex101: build, test, and debug regex](https://regex101.com/)

  &nbsp;

  This is how RegEx can be used in PowerShell. The `$Matches` variable contains the result with the `-match` operator was successful.

  &nbsp;

  ```powershell
  $ipConfigContent = @'
  Windows IP Configuration
  
  Host Name . . . . . . . . . . . . : raandree1
  Primary Dns Suffix  . . . . . . . : 
  Node Type . . . . . . . . . . . . : Hybrid
  IP Routing Enabled. . . . . . . . : No
  WINS Proxy Enabled. . . . . . . . : No
  
  Ethernet adapter vEthernet (DscWorkshop):
  
  Connection-specific DNS Suffix  . : 
  Description . . . . . . . . . . . : Hyper-V Virtual   Ethernet Adapter #2
  Physical Address. . . . . . . . . : 00-15-5D-BC-C3-29
  DHCP Enabled. . . . . . . . . . . : No
  Autoconfiguration Enabled . . . . : Yes
  Link-local IPv6 Address . . . . . :   fe80::8dc2:8da2:dce4:9e7%3(Preferred) 
  IPv4 Address. . . . . . . . . . . : 192.168.111.1  (Preferred) 
  Subnet Mask . . . . . . . . . . . : 255.255.255.0
  Default Gateway . . . . . . . . . : 
  NetBIOS over Tcpip. . . . . . . . : Enabled
  '@
  
  $pattern = 'IPv4 Address[:\.\s]+(?<IpAddress>\d+\.\d+\.\d  +\.\d+)'
  if ($ipConfigContent -match $pattern)
  {
      $Matches.IpAddress
  }
  ```

  &nbsp;
  
  The .net equivalent to the `-match` operator is the `[regex]::Match`  method.

- ### Different ways of filtering lists

    ```powershell
    $a = 1..10
    $a -like 7

    $a | Where-Object { $_ -gt 7 }

    $a | ForEach-Object { if ($_ -gt 7) { $_ } }

    $a.Where({ $_ -gt 7 })
    ```

    &nbsp;

- ### PSCustomObjects

    Hashtables are a good way to structure data, but working with them is not as comfortable as working with objects. This is an example of converting hashtables to one a number of `PSCustomObjects`.

    ```powershell    
    $h = @{
        Surname = 'Doe'
        Givenname = 'John'
        Department = 'IT'
    }, @{
        Surname = 'Smith'
        Givenname = 'Susan'
        Department = 'IT'
    }

    $h.Keys
    $h.Values
    $h.Givenname

    $h | ft givenname,surname #does not work as the hashtable does not provide these propertires

    $data = $h | ForEach-Object { [pscustomobject]$_ }

    $h = [pscustomobject]@{
        Surname = 'Doe'
        Givenname = 'John'
        Department = 'IT'
    }, [pscustomobject]@{
        Surname = 'Smith'
        Givenname = 'Susan'
        Department = 'IT'
    } 
    ```

    &nbsp;

- ### ps1xml Formatters

    The Format.ps1xml files in PowerShell define the default display of objects in the PowerShell console. You can create your own Format.ps1xml files to change the display of objects or to define default displays for new object types that you create in PowerShell ([about_Format.ps1xml](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_format.ps1xml?view=powershell-5.1)).

    &nbsp;

    ```powershell
    $people = Import-Csv .\People.csv -Delimiter ';'
    $people | ForEach-Object { $_.PSObject.TypeNames.Insert(0,'People') }
    Update-FormatData .\People.format.ps1xml
    ```

    > Note: The files for this code are available in [ps1xml](Assets/ps1xml/).

    &nbsp;

- ### AST (Abstract Syntax Tree)

    The PowerShell AST essentially breaks down the code into a hierarchical tree with each element representing a part of the tree, making the scripts self aware.

  - [How to Use the PowerShell AST to Get Meta with Your Scripts](https://adamtheautomator.com/powershell-ast/)
  - [Advanced Tokenizing PowerShell Scripts](https://powershell.one/powershell-internals/parsing-and-tokenization/advanced-tokenizer)

    &nbsp;

    ```powershell
    $tokens = Get-PSOneToken -Path D:\Untitled5.ps1 -ErrorAction Stop
    $tokens[0].Tokens | Where-Object { ($_.TokenFlags -band 'Keyword') -eq 'Keyword' }

    $tokens = $null
    $parseErrors = $null
    $ast = [System.Management.Automation.Language.Parser]::ParseFile('D:\Untitled5.ps1', [ref]$tokens, [ref]$parseErrors)
    ```

    &nbsp;

- ### PowerShell Execution Policy

    The PowerShell Execution Policy is not a security feature: [15 Ways to Bypass the PowerShell Execution Policy](https://www.netspi.com/blog/technical/network-penetration-testing/15-ways-to-bypass-the-powershell-execution-policy/). Method 12 most effective:

    ```powershell
    function Disable-ExecutionPolicy {(
        $ctx = $executioncontext.gettype().getfield("_context","nonpublic,instance").getvalue( $executioncontext)).gettype().getfield("_authorizationManager","nonpublic,instance").setvalue($ctx, (new-object System.Management.Automation.AuthorizationManager "Microsoft.PowerShell"))
        }
        
        Disable-ExecutionPolicy  .runme.ps1
    }
    ```

    &nbsp;

- ### REST API

    We covered how to use the cmdlet `Invoke-RestMethod` by talking to the REST Api of Azure DevOps Server. The article [Create a Pipeline with the Azure DevOps API](https://blog.johnnyreilly.com/2021/05/08/create-pipeline-with-azure-devops-api/) and the API documentation was helpful to create the following code samples.


    ```powershell
    #List repositories
    $org = 'AutomatedLab'
    $project = 'DscConfig.Demo'
    $pat = 'eifrqrgvxtwbf2ox5ouxapskm7ivko6rb7ugebz3djnit76tixvq'
    $patBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(('{0}:{1}' -f 'PAT', $BasicAuthPAT)))
    $headers = @{
        AUTHORIZATION = 'basic {0}' -f $patBase64
    }

    $uri = 'https://dscdo01.contoso.com:8080/{0}/{1}/_apis/git/repositories?api-version=4.1' -f $org, $project

    $response = Invoke-RestMethod -Uri $uri -Method Get -ContentType "application/json" -Headers $headers


    #List pipelines
    #https://learn.microsoft.com/en-us/rest/api/azure/devops/pipelines/pipelines/list?view=azure-devops-rest-6.0
    $org = 'AutomatedLab'
    $project = 'DscConfig.Demo'
    $pat = 'eifrqrgvxtwbf2ox5ouxapskm7ivko6rb7ugebz3djnit76tixvq'
    $patBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(('{0}:{1}' -f 'PAT', $BasicAuthPAT)))
    $headers = @{
        AUTHORIZATION = 'basic {0}' -f $patBase64
    }

    $uri = 'https://dscdo01.contoso.com:8080/{0}/{1}/_apis/pipelines?api-version=6.0-preview.1' -f $org, $project

    $response = Invoke-RestMethod -Uri $uri -Method Get -ContentType "application/json" -Headers $headers

    #Get pipeline
    #https://learn.microsoft.com/en-us/rest/api/azure/devops/pipelines/pipelines/get?view=azure-devops-rest-6.0
    $org = 'AutomatedLab'
    $project = 'DscConfig.Demo'
    $pat = 'eifrqrgvxtwbf2ox5ouxapskm7ivko6rb7ugebz3djnit76tixvq'
    $patBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(('{0}:{1}' -f 'PAT', $BasicAuthPAT)))
    $headers = @{
        AUTHORIZATION = 'basic {0}' -f $patBase64
    }

    #note the pipeline ID as a parameter in the uri
    $uri = 'https://dscdo01.contoso.com:8080/{0}/{1}/_apis/pipelines/1?api-version=6.0-preview.1' -f $org, $project

    $response = Invoke-RestMethod -Uri $uri -Method Get -ContentType "application/json" -Headers $headers

    # Create Pipeline
    #https://learn.microsoft.com/en-us/rest/api/azure/devops/git/repositories/list?view=azure-devops-rest-4.1&tabs=HTTP
    $org = 'AutomatedLab'
    $project = 'DscConfig.Demo'
    $pat = 'eifrqrgvxtwbf2ox5ouxapskm7ivko6rb7ugebz3djnit76tixvq'
    $patBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(('{0}:{1}' -f 'PAT', $BasicAuthPAT)))
    $headers = @{
        AUTHORIZATION = 'basic {0}' -f $patBase64
    }

    $uri = 'https://dscdo01.contoso.com:8080/{0}/{1}/_apis/pipelines?api-version=6.0-preview.1' -f $org, $project

    $body = @{
        folder = ''
        name = "pipeline-made-by-api"
        configuration = @{
            type = 'yaml'
            path = '/azure-pipelines On-Prem.yml'
            repository = @{
                id = '379c50e4-ef90-4d00-8f81-a85535661b2a'
                name = 'DscConfig.Demo'
                type = 'azureReposGit'
            }
        } 
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $uri -Method Post -ContentType 'application/json; charset=utf-8' -Headers $headers -Body $body
    ```

- ### Slatting

    Splatting is a method of passing a collection of parameter values to a command as a unit. PowerShell associates each value in the collection with a command parameter. Splatted parameter values are stored in named splatting variables, which look like standard variables, but begin with an At symbol (`@`) instead of a dollar sign (`$`). The At symbol tells PowerShell that you are passing a collection of values, instead of a single value. ([about_Splatting](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_splatting?view=powershell-7.2)])

    ```powershell
    $org = 'AutomatedLab'
    $project = 'DscConfig.Demo'
    $pat = 'eifrqrgvxtwbf2ox5ouxapskm7ivko6rb7ugebz3djnit76tixvq'
    $patBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(('{0}:{1}' -f 'PAT', $BasicAuthPAT)))
    $headers = @{
        AUTHORIZATION = 'basic {0}' -f $patBase64
    }

    #note the pipeline ID as a parameter in the uri
    $uri = 'https://dscdo01.contoso.com:8080/{0}/{1}/_apis/pipelines/1?api-version=6.0-preview.1' -f $org, $project


    $param = @{
        Uri = $uri
        Method = 'Get'
        ContentType = 'application/json'
        Headers = $headers
    }

    $response = Invoke-RestMethod @param
    ```
