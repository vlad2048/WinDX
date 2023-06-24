using DynamicData;
using PowBasics.CollectionsExt;

namespace UserEvents.Utils;

static class AddDelExts
{
	public static (T[] adds, T[] dels) GetAddDels<T>(this IObservableList<T> list, T[] arr) => (
		arr.WhereNotToArray(list.Items.Contains),
		arr.WhereToArray(list.Items.Contains)
	);
}