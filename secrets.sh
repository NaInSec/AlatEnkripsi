#!/bin/bash
# Catanya : echo "Ini adalah pesan rahasia yang akan dienkripsi." | ./makepaste.sh
# Sumber : https://defuse.ca/pastebin.htm

# Membuat sebuah kata sandi acak dengan menggunakan gpg
PASSWORD=$(gpg --gen-random 2 16 | base64)

# Mengenkripsi teks dengan gpg menggunakan kata sandi acak yang telah dibuat,
# kemudian mengunggahnya ke defuse.ca's pastebin menggunakan curl
URL=$(                                                      \
        gpg --passphrase $PASSWORD -c -a |                  \
        curl -s -d "jscrypt=no" -d "lifetime=864000"        \
        -d "shorturl=yes" --data-urlencode "paste@-"        \
        https://defuse.ca/bin/add.php -D - |                \
        grep Location | cut -d " " -f 2 | tr -d '\r\n'      \
)

# Mencetak perintah untuk mengunduh dan mendekripsi teks dari pastebin menggunakan wget dan gpg
echo "wget $URL?raw=true -q -O - | gpg -d -q --passphrase $PASSWORD"

