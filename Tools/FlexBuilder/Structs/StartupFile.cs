namespace FlexBuilder.Structs;

sealed record StartupFile(
	string Filename,
	bool DeleteAfterOpen
);