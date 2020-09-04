# dfstest

## Algorithm description:

1. Need to get the size of a file and split the file into N logical chunks, where N = 100 in our case
2. Need to start N threads and ask for InputStream with the proper offset
3. Wait for all threads to finish their work and return results
4. Need to calculate the maximum of "maximum number of words" returned by every thread (remote worker)
5. After all remote workers finishes need to calculate the max based on return data:
    number of words in the last line + number of words in the first line of next worker (-1 if there was no firt or last space)

Every remote worker should to the following:

1. Start to read data from the inputstream with the provided offset and length
2. Calculate the number of words at the first line
3. Calculate the number of words at the current line and get max
4. Calculate the number of words at the last line
5. Return the result:

struct {
    do we have a space* at the beginning?
    number of words in the first line,
    maxinum number of words in lines 2..(last-1)
    number of words in the last line,
    do we have a space* at the end?
}

* "space" should be treated as "delimiter" (\n,\r,space,tab)
