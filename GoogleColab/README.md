# Sentiment Analysis Model (Google Colab)

This Google Colab notebook trains and evaluates multiple machine learning models on a YouTube comments sentiment dataset. The goal is to classify comments into **Negative**, **Neutral**, or **Positive** sentiment classes.

---

## Dataset

The project uses the [YouTube Comments Sentiment Dataset](https://www.kaggle.com/datasets/amaanpoonawala/youtube-comments-sentiment-dataset?resource=download).

### How to Download

1. Visit the dataset:  
 [https://www.kaggle.com/datasets/amaanpoonawala/youtube-comments-sentiment-dataset](https://www.kaggle.com/datasets/amaanpoonawala/youtube-comments-sentiment-dataset?resource=download)

2. Click **Download** (you'll need a Kaggle account).

3. Upload `youtube_comments.csv` to your **Google Drive** under: MyDrive/Colab Notebooks/

---

## Models Trained

| Model              | Accuracy |
|--------------------|----------|
| Logistic Regression| 67.76%   |
| Linear SVC         | 67.70%   |
| Naive Bayes        | 63.22%   |
| Random Forest      | 57.13%   |

**Best Model**: **Logistic Regression**

---

## Preprocessing Steps

- Remove mentions, links, hashtags
- Convert emojis using `emoji.demojize`
- Lowercase and strip extra spaces

---

## Model Usage

You can test the model inside the notebook using the `test_input_text()` function:

---

## Exporting the Model

The notebook saves the best model (`Logistic Regression`) and vectorizer using `pickle`.

---

## How to Run the Notebook

1. Open [Google Colab](https://colab.research.google.com)
2. Upload `sa_model.ipynb` and the dataset
3. Mount your Google Drive
4. Install dependencies
5. Run all cells in order.

