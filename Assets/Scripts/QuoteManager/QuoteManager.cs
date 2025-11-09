using UnityEngine;
using System.Collections;
using TMPro; // For TextMeshPro

public class QuoteManager : MonoBehaviour {
    [Header("UI Reference")]
    [SerializeField]
    private TextMeshProUGUI quoteText; // Drag your QuoteText object here

    [Header("Quote Content")]
    [SerializeField]
    [TextArea(3, 5)] // Makes it easier to type in the Inspector
    private string[] quotes; // Your list of quotes

    [Header("Timing Settings")]
    [SerializeField]
    private float minQuoteInterval = 30f; // Min time between quotes

    [SerializeField]
    private float maxQuoteInterval = 60f; // Max time

    [SerializeField]
    private float quoteFadeDuration = 2f; // How long to fade in/out

    [SerializeField]
    private float quoteDisplayDuration = 5f; // How long to show the quote

    void Start() {
        if (quoteText == null) {
            Debug.LogError("QuoteText is not assigned in the Inspector!");
            return;
        }

        // Set initial text color to fully transparent
        quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, 0);

        // Start the quote loop
        StartCoroutine(QuoteLoop());
    }

    // This coroutine runs forever, deciding WHEN to show the next quote
    private IEnumerator QuoteLoop() {
        // Wait for a few seconds at the start before the first quote
        yield return new WaitForSeconds(10f);

        while (true) // Loop forever
        {
            if (quotes.Length == 0) {
                Debug.LogWarning("No quotes are assigned in the QuoteManager.");
                yield return new WaitForSeconds(60f); // Wait a long time before checking again
                continue; // Skip the rest of the loop
            }

            // Pick a random quote
            string randomQuote = quotes[Random.Range(0, quotes.Length)];

            // Call the coroutine to show the quote AND wait for it to finish
            yield return StartCoroutine(ShowQuote(randomQuote));

            // Wait for a random time before showing the next one
            float waitTime = Random.Range(minQuoteInterval, maxQuoteInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // This coroutine handles the fade-in, display, and fade-out
    private IEnumerator ShowQuote(string quote) {
        quoteText.text = quote;
        float timer = 0f;
        Color color = quoteText.color;

        // --- FADE IN ---
        while (timer < quoteFadeDuration) {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / quoteFadeDuration);
            quoteText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null; // Wait for one frame
        }
        quoteText.color = new Color(color.r, color.g, color.b, 1f);

        // --- HOLD ---
        yield return new WaitForSeconds(quoteDisplayDuration);

        // --- FADE OUT ---
        timer = 0f; // Reset timer
        while (timer < quoteFadeDuration) {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / quoteFadeDuration);
            quoteText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null; // Wait for one frame
        }
        quoteText.color = new Color(color.r, color.g, color.b, 0f);
    }
}