import requests
from flask import Flask, render_template, request, redirect, url_for, session, g, send_from_directory
app = Flask(__name__, static_folder='static')
app.debug = True
app.secret_key = "asdhlADlkjdas789daSHkasjllasd7SDJAKL7jlk"
import json

@app.route('/robots.txt')
def static_from_root():
    return send_from_directory(app.static_folder, request.path[1:])

@app.route('/')
@app.route('/<pid>')
def index(pid=-1):
    data = requests.get('http://localhost:5080/Comic?page=1&step=80').text
    #data is json string
    #convert 
    data = json.loads(data)
    
    return render_template('index.html',data = data["data"])

@app.route('/about')
def about():
    return render_template('about.html',)

if __name__ == '__main__':

    app.run()
