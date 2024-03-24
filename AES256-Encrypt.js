// node Encrypt-NTLM.js

const readline = require('readline').createInterface({
    input: process.stdin,
    output: process.stdout
});

const fs = require('fs');
const crypto = require('crypto');

// Meminta input folder tujuan
readline.question('Masukkan Folder Tujuan: ', (folderPath) => {
    // Meminta input file
    readline.question('Masukkan path file (pisahkan dengan koma jika lebih dari satu): ', (filePaths) => {
        // Meminta input password
        readline.question('Masukkan password enkripsi: ', (password) => {
            // Proses enkripsi setiap file di dalam folder
            filePaths.split(',').forEach((filePath) => {
                // Ambil nama file dari path lengkap
                const fileName = filePath.split('/').pop();

                // Membaca isi file
                fs.readFile(filePath, (err, data) => {
                    if (err) {
                        console.error(`Error reading file ${filePath}: ${err}`);
                        return;
                    }

                    // Mengenkripsi file dengan password
                    const cipher = crypto.createCipher('aes-256-cbc', password);
                    const encryptedData = Buffer.concat([cipher.update(data), cipher.final()]);

                    // Menyimpan file terenkripsi
                    const encryptedFilePath = `${folderPath}/${fileName}.enc`;
                    fs.writeFile(encryptedFilePath, encryptedData, (err) => {
                        if (err) {
                            console.error(`Error writing encrypted file ${encryptedFilePath}: ${err}`);
                            return;
                        }

                        console.log(`File ${filePath} telah dienkripsi dan disimpan di ${encryptedFilePath}`);
                    });

                    // Menghapus file asli
                    fs.unlink(filePath, (err) => {
                        if (err) {
                            console.error(`Error deleting original file ${filePath}: ${err}`);
                            return;
                        }
                    });
                });
            });

            // Menutup interface readline setelah selesai
            readline.close();
        });
    });
});
