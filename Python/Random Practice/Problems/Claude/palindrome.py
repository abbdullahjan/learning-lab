def isPalindrome(str):
    str = str.upper()
    str = str.replace(" ","")
    reverse = str[::-1]

    if str == reverse:
        return True
    return False

print(isPalindrome("A man a plan a canal Panama"))

