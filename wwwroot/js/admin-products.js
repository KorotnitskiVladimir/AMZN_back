document.addEventListener("DOMContentLoaded", function () {
    const deleteModalElement = document.getElementById("adminDeleteConfirmModal");
    if (!deleteModalElement) {
        return;
    }

    const deleteForm = document.getElementById("adminDeleteConfirmForm");
    const productTitleElement = deleteModalElement.querySelector(".js-admin-delete-product-title");

    deleteModalElement.addEventListener("show.bs.modal", function (event) {
        const triggerButton = event.relatedTarget;
        if (!triggerButton || !deleteForm) {
            return;
        }

        const deleteUrl = triggerButton.getAttribute("data-delete-url");
        const productTitle = triggerButton.getAttribute("data-product-title");

        deleteForm.action = deleteUrl || "";

        if (productTitleElement) {
            productTitleElement.textContent = productTitle || "this product";
        }
    });

    deleteModalElement.addEventListener("hidden.bs.modal", function () {
        if (deleteForm) {
            deleteForm.removeAttribute("action");
        }

        if (productTitleElement) {
            productTitleElement.textContent = "this product";
        }
    });
});
