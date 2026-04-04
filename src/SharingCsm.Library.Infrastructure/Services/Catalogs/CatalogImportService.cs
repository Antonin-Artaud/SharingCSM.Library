using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Infrastructure.Services.Daos.Books;
using System.Runtime.CompilerServices;

namespace SharingCsm.Library.Infrastructure.Services.Catalogs;

public sealed class CatalogImportService : ICatalogImportService
{
	public async IAsyncEnumerable<BookImportDao> ImportBadAsync(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		using var reader = new StreamReader(
			stream,
			encoding: System.Text.Encoding.UTF8,
			detectEncodingFromByteOrderMarks: true,
			bufferSize: 1024,
			leaveOpen: true);

		string? line;

		while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
		{
			// .Split() crée un TOUT NOUVEAU tableau string[] en mémoire (Tas / Heap).
			// De plus, il crée 3 nouvelles chaînes (string) pour l'ISBN, le Titre et la Catégorie.
			// Si le CSV a 100 000 lignes, on vient de créer 400 000 objets inutiles qui seront 
			// détruits par le Garbage Collector la milliseconde suivante.
			string[] parts = line.Split(';');

			if (parts.Length != 3) continue;

			BookCategory bookCategory = Enum.TryParse<BookCategory>(parts[2], out var parsedCategory) ? parsedCategory : BookCategory.Unknown;

			// Guid.Parse alloue encore de la mémoire interne pour parser la string.
			yield return new BookImportDao(
				Guid.Parse(parts[0]),
				parts[1],
				bookCategory);
		}
	}

	public async IAsyncEnumerable<BookImportDao> ImportFastAsync(
		Stream stream,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		using var reader = new StreamReader(
			stream,
			encoding: System.Text.Encoding.UTF8,
			detectEncodingFromByteOrderMarks: true,
			bufferSize: 1024,
			leaveOpen: true);

		string? line;

		while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
		{
			// On transforme la ligne en ReadOnlySpan. C'est une simple "fenêtre" 
			// qui regarde la mémoire existante. ZÉRO ALLOCATION.
			ReadOnlySpan<char> lineSpan = line.AsSpan();

			// 1. Extraction de l'ISBN
			int firstSeparator = lineSpan.IndexOf(';');
			if (firstSeparator == -1) continue;

			ReadOnlySpan<char> isbnSpan = lineSpan.Slice(0, firstSeparator);
			lineSpan = lineSpan.Slice(firstSeparator + 1); // On avance la fenêtre

			// 2. Extraction du Titre
			int secondSeparator = lineSpan.IndexOf(';');
			if (secondSeparator == -1) continue;

			ReadOnlySpan<char> titleSpan = lineSpan.Slice(0, secondSeparator);

			// 3. Extraction de la Catégorie (le reste de la ligne)
			ReadOnlySpan<char> categorySpan = lineSpan.Slice(secondSeparator + 1);

			BookCategory bookCategory = Enum.TryParse<BookCategory>(categorySpan, out var parsedCategory) ? parsedCategory : BookCategory.Unknown;

			// LA MAGIE C# MODERNE :
			// Guid.Parse est capable de lire nativement un Span sans créer de string !
			// Pour le titre, on fait un .ToString() uniquement 
			// sur la donnée finale dont on a besoin, sans aucun tableau intermédiaire.
			yield return new BookImportDao(
				Guid.Parse(isbnSpan),
				titleSpan.ToString(),
				bookCategory);
		}
	}
}