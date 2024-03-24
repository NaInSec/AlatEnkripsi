#!/bin/bash
# Carabya : ./supersecrets.sh secretsfile.txt
# Sumber : https://defuse.ca/pastebin.htm

# Memeriksa apakah argumen file telah diberikan
if [ $# -eq 0 ]; then
    echo "Usage: $0 <file>"
    exit 1
fi

# Memeriksa apakah file yang diberikan ada dan dapat dibaca
if [ ! -r "$1" ]; then
    echo "Error: File $1 not found or not readable."
    exit 1
fi

# Membuat sebuah kata sandi acak dengan menggunakan gpg
PASSWORD=$(gpg --gen-random 2 16 | base64)

# Mengenkripsi teks dari file dengan gpg menggunakan kata sandi acak yang telah dibuat,
# kemudian mengunggahnya ke defuse.ca's pastebin menggunakan curl
URL=$(                                                      \
        gpg --passphrase $PASSWORD -c -a < "$1" |           \
        curl -s -d "jscrypt=no" -d "lifetime=864000"        \
        -d "shorturl=yes" --data-urlencode "paste@-"        \
        https://defuse.ca/bin/add.php -D - |                \
        grep Location | cut -d " " -f 2 | tr -d '\r\n'      \
)

# Mencetak perintah untuk mengunduh dan mendekripsi teks dari pastebin menggunakan wget dan gpg
echo "wget $URL?raw=true -q -O - | gpg -d -q --passphrase $PASSWORD"

