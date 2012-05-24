param($installPath, $toolsPath, $package, $project)

$fi = new-object io.fileinfo($project.FullName)
$rootPath = $fi.Directory.FullName

mv "$rootPath\captcha.js" "$rootPath\Scripts\captcha.js"

($project.ProjectItems | ? {$_.Name -eq 'captcha.js'}).Remove()
($project.ProjectItems | ? {$_.Name -eq 'Scripts'}).ProjectItems.AddFromFile("$rootPath\Scripts\captcha.js")
