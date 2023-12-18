Finds patterns in a CSV file with one million rows, and deletes lines matching a certain input pattern.
Optimized to run in 10 to 30 seconds.

Description - 

Records Deletion by Chunk
This time, we are playing around with single digits, SET 5 and SET 6. Each set has individual number from 0 to 9. The logic to remove the records no longer search by “so long I found” but it has to be in chunk. Let me explain and if you have any question, please check with me before you start, so you don’t have to rework a few times again.

SET 5: 99999 records from 00000 to 99999
SET 6: 999999 records from 000000 to 999999

STEPS
1.Import listing file, either Set 5 or Set 6
2.Import blacklisted file, either Set 5 or Set 6
3.Allow me to input the “chunk” pattern, eg: 2,3,4,5 or 6
4.Based on my input, the process will start to split ALL the blacklisted list by chunk

Example of temporary output of SET 6

If I request to split the chunk by 2
Blacklisted	It will list out every 2 number according to the sequence
893424	89	93	34	42	24
475380	47	75	53	38	80
371000	37	71	10	00	00

If I request to split the chunk by 3
Blacklisted	It will list out every 3 number according to the sequence
893424	893	934	342	424
475380	475	753	538	380
371000	371	710	100	000

If I request to split the chunk by 4
Blacklisted	It will list out every 4 number according to the sequence
893424	8934	9342	3424
475380	4753	7538	5380
371000	3710	7100	1000

If I request to split the chunk by 5
Blacklisted	It will list out every 5 number according to the sequence
893424	89342	93424
475380	47538	75380
371000	37100	71000

If I request to split the chunk by 6
Blacklisted	It will list out every 6 number according to the sequence
893424	893424
475380	475380
371000	371000

5.The process then allowed me to start delete the listing records that MATCHED the chunk
6.Eg. If I wish to delete any records that found by splitting chunk of 4, so long the listings FOUND the below; they should be remove. 



Example of records to be delete, I am only using 8934, 4753 and 3710 here…the process should also look for 9342, 7538, 7100, 3424, 5380, 1000 and so on… (the chunk from the blacklisted list), then go to the MAIN FILE listing and delete the below

008934
018934
038934
054753
475388
754753
437103
633710
837101
Etc ……

All these records should be auto removed

7.There is a separate module within the same File Generator, allowed me just to list out records based on input
8.Eg: If I input number “5”, it will list out all the records that contains “5”, it can be anything so long there is 5 in it. This is super straight forward. Usually, I will need to find the list after I done removing the records by chunks.
9.Eg: 000005, 055256, 512547, 125458 
10.The FILE should be flexible for me able to use both Set 5 or Set 6, just like what you have done for me on Set 36, 50, 55 and 58.
