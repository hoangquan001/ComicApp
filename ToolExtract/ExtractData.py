pip install jmespath httpx parsel
%%timeit
import json
import httpx
import jmespath
from parsel import Selector

# establish HTTP client and to prevent being instantly banned lets set some browser-like headers
session = httpx.Client()

# send a request to the target website
request = session.get(
    "https://doctruyenonline.vn/truyen-tranh/tu-tien-o-the-gioi-sieu-nang-luc"
)
selector = Selector(text=request.text)
json_data = selector.css("script[type='application/json']::text").get()
data = json.loads(json_data)



# JMESPath expressions
expression = jmespath.compile(
    """
    {
      "Name": props.pageProps.comic[0].nameComic,
      "URL": props.pageProps.comic[0].urlComic,
      "Description": props.pageProps.comic[0].descriptComic,
      "CoverImage":   props.pageProps.comic[0].imgComic,
      "create_date":  props.pageProps.comic[0].create_date
      "totalChapter": props.pageProps.comic[0].totalChapter
      "statusComic": props.pageProps.comic[0].statusComic
      "Category":   props.pageProps.comic[0].category[*].{id:id_cate, name: name_cate},
      "listChapter": props.pageProps.listChapter[].{urlChapter:urlChapter, dateUpdateChapter: dateUpdateChapter}
    }
    """
)

# Apply JMESPath expression
flat_data = expression.search(data)

# Print the restructured data
print(json.dumps(flat_data, indent=2))



####
from bs4 import BeautifulSoup

# Assuming we have an HTML document in the 'html_content' variable
soup = BeautifulSoup(html_content, 'html.parser')

# Find the desired element and extract its text using get_text()
element = soup.find_all('span',itemprop="name")

#get_text with strip set to true
for e in element:
  print(e) 