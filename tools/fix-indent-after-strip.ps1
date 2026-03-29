$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\Assets")).Path
$files = Get-ChildItem -Path $root -Filter *.cs -Recurse
foreach ($path in $files) {
  $lines = [IO.File]::ReadAllLines($path.FullName)
  $i = 0
  while ($i -lt $lines.Length -and $lines[$i] -match '^\s*using\s') { $i++ }
  while ($i -lt $lines.Length -and [string]::IsNullOrWhiteSpace($lines[$i])) { $i++ }
  if ($i -ge $lines.Length) { continue }
  $out = [System.Collections.Generic.List[string]]::new()
  for ($j = 0; $j -lt $i; $j++) { $out.Add($lines[$j]) }
  for ($j = $i; $j -lt $lines.Length; $j++) {
    $line = $lines[$j]
    if ($line.Length -eq 0) { $out.Add(''); continue }
    $out.Add('    ' + $line)
  }
  $text = ($out -join "`n") + "`n"
  [IO.File]::WriteAllText($path.FullName, $text, [System.Text.UTF8Encoding]::new($false))
  Write-Host "indent $($path.Name)"
}
Write-Host "Done."
