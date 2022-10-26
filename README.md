# Itz PowerShell Workshop

## Recommended PowerShell Modules

- [Sampler: Module template with build pipeline and examples, including DSC elements)](https://github.com/gaelcolas/Sampler)
- [AutomatedLab - Lab automation from users for users](https://automatedlab.org)
- [NTFSSecurity: Managing permissions with PowerShell](https://github.com/raandree/NTFSSecurity)
- [ConvertTo-Expression: Serializes an object to a PowerShell expression](https://github.com/iRon7/ConvertTo-Expression)
- [Join-Object: Combines two objects lists based on a related property between them](https://github.com/iRon7/Join-Object)
- [ThreadJob: A PowerShell module for running concurrent jobs based on threads rather than processes](https://github.com/PowerShell/ThreadJob)

## Recommended Resources

- [PowerShell Explained](https://powershellexplained.com)

## Code Samples

- ### ThreadJob are running within the same thread and are more efficient.

```powershell
#Install-Module ThreadJob -Force
$sb = { Get-Random; Start-Sleep -Milliseconds 2000 }

1..100 | ForEach-Object {
    #Start-Job -ScriptBlock $sb
    Start-ThreadJob -ScriptBlock $sb
} | Receive-Job -AutoRemoveJob -Wait

$r.Count
```

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

- ### Offline install of vscode extensions

This code was taken from [DscWorkshop/Lab/20 Lab Customizations.ps1](https://github.com/dsccommunity/DscWorkshop/blob/fdd283fa9a0cee53f1a749352969b387e752a0a5/Lab/20%20Lab%20Customizations.ps1#L166)
```powershell
Copy-LabFileItem -Path $labSources\SoftwarePackages\VSCodeExtensions -ComputerName $devOpsServer
Invoke-LabCommand -ActivityName 'Install VSCode Extensions' -ComputerName $devOpsServer -ScriptBlock {
    dir -Path C:\VSCodeExtensions | ForEach-Object {
        code --install-extension $_.FullName 2>$null #suppressing errors
    }
} -NoDisplay
```

> Note: You can download any VSCode extension from the Visual Studio Marketplace like for example https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell.
