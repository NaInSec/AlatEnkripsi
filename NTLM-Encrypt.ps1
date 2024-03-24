# Meminta input folder tujuan
$folderPath = Read-Host "Masukkan path folder tujuan"

# Meminta input file
$filePaths = Read-Host "Masukkan path file (pisahkan dengan koma jika lebih dari satu)"

# Meminta input password
$password = Read-Host "Masukkan password enkripsi" -AsSecureString

# Ubah password menjadi format yang bisa digunakan untuk enkripsi
$passwordString = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

# Proses enkripsi setiap file di dalam folder
foreach ($filePath in $filePaths.Split(',')) {
    # Ambil nama file dari path lengkap
    $fileName = (Get-Item $filePath).Name
    
    # Mengenkripsi file dengan password
    $encryptedFilePath = "$folderPath\$fileName.enc"
    Encrypt-FileNTLM -Path $filePath -Password $passwordString -OutputPath $encryptedFilePath

    # Menghapus file asli
    Remove-Item $filePath
}

Write-Host "File telah dienkripsi dan dipindahkan ke folder tujuan: $folderPath"

Function Encrypt-FileNTLM {
    param (
        [string]$Path,
        [string]$Password,
        [string]$OutputPath
    )
    
    $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
    $key = $securePassword | ConvertFrom-SecureString -Key (1..16)
    
    $keyByte = [System.Text.Encoding]::Unicode.GetBytes($key)
    $iv = New-Object byte[] 16
    $aes = New-Object System.Security.Cryptography.RijndaelManaged
    $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
    $aes.Padding = [System.Security.Cryptography.PaddingMode]::Zeros
    $aes.BlockSize = 128
    $aes.KeySize = 128
    $aes.Key = $keyByte
    $aes.IV = $iv
    
    $encryptor = $aes.CreateEncryptor($aes.Key, $aes.IV)
    
    $inputFileStream = [System.IO.File]::OpenRead($Path)
    $outputFileStream = [System.IO.File]::Create($OutputPath)
    
    $cryptoStream = New-Object System.Security.Cryptography.CryptoStream $outputFileStream, $encryptor, [System.Security.Cryptography.CryptoStreamMode]::Write
    
    $bufferSize = 4096
    $buffer = New-Object byte[] $bufferSize
    $readBytes = 0
    
    while (($readBytes = $inputFileStream.Read($buffer, 0, $bufferSize)) -gt 0) {
        $cryptoStream.Write($buffer, 0, $readBytes)
    }
    
    $cryptoStream.FlushFinalBlock()
    
    $inputFileStream.Close()
    $outputFileStream.Close()
    $cryptoStream.Close()
}

