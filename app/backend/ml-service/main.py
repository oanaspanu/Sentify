# sentiment_api.py
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import pickle, emoji, re
from sklearn.feature_extraction.text import TfidfVectorizer

app = FastAPI()

with open("sa_model.pkl", "rb") as f:
    model = pickle.load(f)
with open("tfidf_vectorizer.pkl", "rb") as f:
    vectorizer = pickle.load(f)

def preprocess(text: str):
    text = emoji.demojize(text, delimiters=(" ", " "))
    text = re.sub(r'\s+', ' ', text).lower()
    return text.strip()

class TextRequest(BaseModel):
    text: str

@app.post("/predict")
def predict_sentiment(request: TextRequest):
    if not request.text.strip():
        raise HTTPException(status_code=400, detail="Text input required")
    
    cleaned = preprocess(request.text)
    vectorized = vectorizer.transform([cleaned])
    prediction = model.predict(vectorized)[0]
    
    return {"sentiment": int(prediction)}
