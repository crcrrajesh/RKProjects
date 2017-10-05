import nester

def printfile(filename):
    data=open(filename)
    for line in data:
        print(line,end='')
    data.close()

def main2():
    movies = [
        "The Holy Grail", 1975, "Terry Jones & Terry Gilliam", 91,
        ["Graham Chapman",
         ["Michael Palin", "John Cleese", "Terry Gilliam", "Eric Idle", "Terry Jones"]]]
   # nester.print_lol(movies)
    printfile("calculator.py")
    filename="calculator.py"
    writefile(filename)


def writefile(filename):
    data = open(filename)
    outdata = open("ne.txt", "w")
    for line in data:
        print(line, file=outdata)
    outdata.close()


if __name__ == "__main__": main2()



