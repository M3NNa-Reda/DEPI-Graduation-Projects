// Client-side helpers for the storefront.

// Filters the product grid as the customer types.
(function () {
  const input = document.getElementById('productSearch');
  if (!input) return;

  const items = Array.from(document.querySelectorAll('[data-product-name]'));
  const empty = document.getElementById('noSearchResults');

  input.addEventListener('input', function () {
    const term = input.value.trim().toLowerCase();
    let visible = 0;

    items.forEach(function (item) {
      const match = item.dataset.productName.includes(term);
      item.classList.toggle('d-none', !match);
      if (match) visible++;
    });

    if (empty) empty.classList.toggle('d-none', visible !== 0);
  });
})();

// Swaps the main picture on the product page when a thumbnail is clicked.
(function () {
  const main = document.getElementById('galleryMain');
  const thumbs = document.querySelectorAll('.gallery-thumb');
  if (!main || thumbs.length === 0) return;

  thumbs.forEach(function (thumb) {
    thumb.addEventListener('click', function () {
      main.src = thumb.dataset.full;
      thumbs.forEach(function (t) { t.classList.remove('is-active'); });
      thumb.classList.add('is-active');
    });
  });
})();
