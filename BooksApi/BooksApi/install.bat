sc create "BooksApi" binPath="%CD%\BooksApi.exe" start=auto
sc failure "BooksApi" reset=0 actions=restart/60000/restart/60000/restart/60000
net start "BooksApi"
