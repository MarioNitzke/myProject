document.addEventListener('DOMContentLoaded', function () {
	const dateOfBirthInput = document.getElementById('DateOfBirth');
	const ssnInput = document.getElementById('SocialSecurityNumber');

	// Funkce pro generování rodného čísla
	function generateSocialSecurityNumber() {
		const dateOfBirth = dateOfBirthInput.value;

		// Pokud není datum zadáno, zachováme existující část za lomítkem a nebudeme generovat
		if (!dateOfBirth) {
			return;
		}

		// Rozdělení data na části (formát "yyyy-MM-dd")
		const dateParts = dateOfBirth.split('-');
		if (dateParts.length !== 3) {
			// Pokud není datum ve správném formátu, neaktualizujeme rodné číslo
			return;
		}

		const year = dateParts[0].substring(2); // Poslední dvě číslice roku
		const month = dateParts[1];
		const day = dateParts[2];

		// Základní část rodného čísla
		const socialSecurityNumberBase = `${year}${month}${day}/`;

		// Pokud existuje část za lomítkem, zachováme ji, jinak ponecháme prázdnou
		const existingRandomPart = ssnInput.value.split('/')[1] || '';

		// Aktualizace pole rodného čísla
		ssnInput.value = `${socialSecurityNumberBase}${existingRandomPart}`;
	}

	// Spuštění funkce při změně data narození
	dateOfBirthInput.addEventListener('input', generateSocialSecurityNumber);

	// Při načtení stránky znovu vygenerujte rodné číslo, pokud je datum vyplněné
	if (dateOfBirthInput.value) {
		generateSocialSecurityNumber();
	}
});