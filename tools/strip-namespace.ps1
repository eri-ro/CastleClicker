$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\Assets")).Path
$files = Get-ChildItem -Path $root -Filter *.cs -Recurse
foreach ($path in $files) {
  $c = Get-Content -LiteralPath $path.FullName -Raw -Encoding UTF8
  if ($c -notmatch 'namespace\s+CastleClicker') { continue }
  $c2 = [regex]::Replace($c, '(?ms)^\s*namespace\s+CastleClicker\s*$\r?\n\s*\{\s*\r?\n', '', 1)
  if ($c2 -eq $c) {
    Write-Warning "No strip: $($path.FullName)"
    continue
  }
  $open = ([regex]::Matches($c2, '\{')).Count
  $close = ([regex]::Matches($c2, '\}')).Count
  $lines = [System.Collections.Generic.List[string]]::new()
  foreach ($ln in ($c2 -split '\r?\n', [StringSplitOptions]::None)) { $lines.Add($ln) }
  while ($close -gt $open -and $lines.Count -gt 0) {
    $last = $lines.Count - 1
    while ($last -ge 0 -and [string]::IsNullOrWhiteSpace($lines[$last])) {
      $lines.RemoveAt($last)
      $last--
    }
    if ($last -lt 0) { break }
    if ($lines[$last].Trim() -eq '}') {
      $lines.RemoveAt($last)
      $text = ($lines -join "`n")
      $open = ([regex]::Matches($text, '\{')).Count
      $close = ([regex]::Matches($text, '\}')).Count
    } else { break }
  }
  $out = foreach ($line in $lines) {
    if ($line.Length -ge 4 -and $line.Substring(0, 4) -eq '    ') { $line.Substring(4) }
    else { $line }
  }
  $final = ($out -join "`n") + "`n"
  [IO.File]::WriteAllText($path.FullName, $final, [System.Text.UTF8Encoding]::new($false))
  Write-Host "OK $($path.Name)"
}
Write-Host "Done."
