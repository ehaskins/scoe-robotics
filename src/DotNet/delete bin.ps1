function main(){
	deleteBin((get-location).ToString())
}

function deleteBin ($path){
	$dirs = [System.IO.DirectoryInfo]::GetDirectory($path).GetDirectories($path)
    foreach ($dir in $dirs){
		if ($dir.Name -eq "bin") {
			Remove-Item $dir -R -WhatIf}
		else{
			deleteBin($dir)}
	}
	
}

main