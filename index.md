# Disk Reader

## Compile

dotnet publish -o ../output -r linux-x64 --self-contained false

## Test

echo "../test-files/hello-world" | nc 127.0.0.1 11000 > hello-world

echo "../test-files/test.txt" | nc 127.0.0.1 11000 > test.txt
