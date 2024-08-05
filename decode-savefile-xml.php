<?php

$encodedSaveFilePath = $argv[1] ?? null;

if ($encodedSaveFilePath === null) {
    fwrite(STDERR, 'File not found.' . PHP_EOL);
    exit(1);
}

if (!is_readable($encodedSaveFilePath)) {
    fwrite(STDERR, 'Could not read file.' . PHP_EOL);
    exit(2);
}

$contents = file_get_contents($encodedSaveFilePath);
$lengthContents = strlen($contents);

$start = 4;

$ouput = [];

$num3 = 33;

for($i = $start; $i < $lengthContents; $i++) {
    //echo ord($contents[$i]), PHP_EOL;
    //continue;
    
    
    $output[] = ord($contents[$i]) ^ $num3;
    //echo end($output), PHP_EOL;
    $num3++;
    if ($num3 > 255) {
        $num3 = 0;
    }
    
}

foreach($output as $byte) {
    echo chr($byte);
}
