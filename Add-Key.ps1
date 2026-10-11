# Run under the service account or as Administrator
Add-Type -AssemblyName System.Security

# --- 1. Name of the encrypted file ---
$fileName = Read-Host "Enter the name of the encrypted key file (e.g. encryption.key.enc)"
if ([string]::IsNullOrWhiteSpace($fileName)) {
    throw "File name cannot be empty."
}
if ($fileName.IndexOfAny([System.IO.Path]::GetInvalidFileNameChars()) -ge 0) {
    throw "File name contains invalid characters."
}

# --- 2. Folder location ---
$folder = Read-Host "Enter the folder to save it in (e.g. C:\ProgramData\MyApp)"
if ([string]::IsNullOrWhiteSpace($folder)) {
    throw "Folder cannot be empty."
}
if (-not (Test-Path -LiteralPath $folder -PathType Container)) {
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    Write-Host "Created folder $folder"
}

$outputPath = Join-Path $folder $fileName

if (Test-Path -LiteralPath $outputPath) {
    $overwrite = Read-Host "$outputPath already exists. Overwrite? (y/n)"
    if ($overwrite -ne 'y') {
        Write-Host "Cancelled."
        return
    }
}

# --- 3. The key (typed input is hidden) ---
$secureKey = Read-Host "Enter the key" -AsSecureString
$bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureKey)
try {
    $plainKey = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
}

if ([string]::IsNullOrEmpty($plainKey)) {
    throw "Key cannot be empty."
}

$bytes = [System.Text.Encoding]::UTF8.GetBytes($plainKey)
if ($bytes.Length -ne 32) {
    Write-Warning "Key is $($bytes.Length) bytes, not 32."
}

# --- Protect using LocalMachine scope ---
$encrypted = [System.Security.Cryptography.ProtectedData]::Protect(
    $bytes,
    $null,
    [System.Security.Cryptography.DataProtectionScope]::LocalMachine
)

# --- Save the encrypted payload to disk ---
[System.IO.File]::WriteAllBytes($outputPath, $encrypted)

# Clean up the plaintext from memory
[Array]::Clear($bytes, 0, $bytes.Length)
Remove-Variable plainKey, bytes, secureKey -ErrorAction SilentlyContinue

Write-Host "Encrypted key saved to $outputPath"