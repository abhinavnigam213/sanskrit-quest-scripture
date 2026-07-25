namespace SanskritQuest.Common.Contracts
{
	public enum Sect
	{
		[DatabaseId(1)] Vaishnavism = 1,
		[DatabaseId(2)] Shaivism = 2,
		[DatabaseId(3)] Shaktism = 3,
		[DatabaseId(4)] Smartism = 4,
		[DatabaseId(5)] Saurism = 5,
		[DatabaseId(6)] Ganapatism = 6,
		[DatabaseId(7)] Kaumaram = 7
	}

	public enum Philosophy
	{
		[DatabaseId(1)] AdvaitaVedanta = 1,
		[DatabaseId(2)] Vishishtadvaita = 2,
		[DatabaseId(3)] Dvaita = 3,
		[DatabaseId(4)] Samkhya = 4,
		[DatabaseId(5)] Yoga = 5,
		[DatabaseId(6)] Nyaya = 6,
		[DatabaseId(7)] Vaisheshika = 7,
		[DatabaseId(8)] Mimamsa = 8,
		[DatabaseId(9)] Bhedabheda = 9,
		[DatabaseId(10)] Shuddhadvaita = 10,
		[DatabaseId(11)] Dvaitadvaita = 11
	}

	public enum Deity
	{
		[DatabaseId(1)] Vishnu = 1,
		[DatabaseId(2)] Krishna = 2,
		[DatabaseId(3)] Shiva = 3,
		[DatabaseId(4)] Devi = 4,
		[DatabaseId(5)] Agni = 5,
		[DatabaseId(6)] Ganesha = 6,
		[DatabaseId(7)] Surya = 7,
		[DatabaseId(8)] Kartikeya = 8,
		[DatabaseId(9)] Indra = 9,
		[DatabaseId(10)] Saraswati = 10,
		[DatabaseId(11)] Lakshmi = 11,
		[DatabaseId(12)] Rama = 12,
		[DatabaseId(13)] Hanuman = 13
	}

	public enum ScriptureCategory
	{
		[DatabaseId(1)] Shruti = 1,
		[DatabaseId(2)] Smriti = 2
	}

	public enum ScriptureClass
	{
		[DatabaseId(1)] Itihasa = 1,
		[DatabaseId(2)] Samhita = 2,
		[DatabaseId(3)] Brahmana = 3,
		[DatabaseId(4)] Aranyaka = 4,
		[DatabaseId(5)] Upanishad = 5,
		[DatabaseId(6)] Purana = 6,
		[DatabaseId(7)] SutraShastra = 7
	}

	public enum Commentator
	{
		[DatabaseId(1)] AdiShankara = 1,
		[DatabaseId(2)] Ramanuja = 2,
		[DatabaseId(3)] Madhvacharya = 3,
		[DatabaseId(4)] Abhinavagupta = 4,
		[DatabaseId(5)] Jnaneshwar = 5,
		[DatabaseId(6)] Vallabhacharya = 6,
		[DatabaseId(7)] Nimbarkacharya = 7
	}

	public enum ScriptureType
	{
		[DatabaseId(1)] BhagavadGita = 1,
		[DatabaseId(2)] ValmikiRamayana = 2
	}
}
