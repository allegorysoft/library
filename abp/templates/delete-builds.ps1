Get-ChildItem .\ -include bin,obj,.idea -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

Get-ChildItem -h .\ -include .vs -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
