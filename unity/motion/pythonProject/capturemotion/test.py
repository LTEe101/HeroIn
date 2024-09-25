import random

def generate_random_hexadecimal(length):
    return ''.join(random.choices('0123456789abcdef', k=length))

random_hex_40 = generate_random_hexadecimal(30)
print(random_hex_40)
