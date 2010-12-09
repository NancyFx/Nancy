properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
}

task default -depends Test

task Test -depends Compile, Clean { 
  Write-Host $testMessage
}

task Compile -depends Clean { 
  Write-Host $compileMessage
}

task Clean { 
  Write-Host $cleanMessage
}