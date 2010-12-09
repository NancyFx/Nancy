task default -depends doStuff
task doStuff {
  Write-Host "Starting to do stuff..."
  Write-Host "Adding stuff... 1 + 1 =" (1+1)
  Write-Host "Stuff done!"
}