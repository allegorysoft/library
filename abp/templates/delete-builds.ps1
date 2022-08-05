Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

Get-ChildItem -h .\ -include .vs -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
