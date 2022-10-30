Add-Type -Path D:\Car\Car\bin\Debug\Car.dll

$c = New-Object My.Car
$c.MaxSpeed = 300

[My.Car]::StandardMaxSpeed