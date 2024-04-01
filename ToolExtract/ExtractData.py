import requests
import re
res = requests.get('https://doctruyenonline.vn/truyen-tranh/toi-se-bi-tru-khu-cung-hoang-de')
text = res.text

file = open('test.html', 'w', encoding='utf-8')

pattern = r"(?is)<head>(.*?)</head>"  # Improved pattern with comments
text=re.sub(pattern, "", text)
pattern = r"(?is)<body[^*]>(.*?)</body>"  # Improved pattern with comments
text=re.sub(pattern, "", text)



file.write(text)

