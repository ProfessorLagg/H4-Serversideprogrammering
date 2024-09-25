using namespace System.IO;
using namespace System.Security.Cryptography;
using namespace System.Text;
using namespace System.Net;

function Get-HexString{
    Param(
        [byte[]]$bytes
    )
    $str = "";
    for($i = 0; $i -lt $bytes.Count; $i++){
        $str += $bytes[$i].ToString("x").PadLeft(2,'0')
    }
    return $str
}

$inPath = "C:\CodeProjects\Skole\H4-Serversideprogrammering\Database\Mock\Users.tsv"
$inLines = [File]::ReadAllLines($inPath)

$users = @([PsCustomObject]@{Login = "admin"; PasswordHash = "admin";})
[HashAlgorithm]$hasher = [SHA384]::Create()
for($i = 1; $i -lt $inLines.Count;$i++){
    
    $line = $inLines[$i]
    $split = $line.Split("`t")
    [byte[]]$pwdBytes = [Encoding]::UTF8.GetBytes($split[1])
    [byte[]]$hashBytes = $hasher.ComputeHash($pwdBytes)
    $users += [PsCustomObject]@{
        Login = $split[0];
        PasswordHash = Get-HexString -bytes $hashBytes
    }
}
$hasher.Dispose()
[WebClient]$client = [WebClient]::new()
foreach($user in $users){
    $jsonString = $user | ConvertTo-Json -Compress
}
$client.Dispose()