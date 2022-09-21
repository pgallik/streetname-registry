namespace StreetNameRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public sealed class StreetNameDbaseRecord : DbaseRecord
    {
        public static readonly StreetNameDbaseSchema Schema = new StreetNameDbaseSchema();

        public DbaseCharacter id { get; }
        public DbaseInt32 straatnmid { get; }
        public DbaseCharacter versieid { get; }
        public DbaseCharacter gemeenteid { get; }
        public DbaseCharacter straatnm { get; }
        public DbaseCharacter homoniemtv { get; }
        public DbaseCharacter status { get; }

        public StreetNameDbaseRecord()
        {
            id = new DbaseCharacter(Schema.id);
            straatnmid = new DbaseInt32(Schema.straatnmid);
            versieid = new DbaseCharacter(Schema.versieid);
            gemeenteid = new DbaseCharacter(Schema.gemeenteid);
            straatnm = new DbaseCharacter(Schema.straatnm);
            homoniemtv = new DbaseCharacter(Schema.homoniemtv);
            status = new DbaseCharacter(Schema.status);

            Values = new DbaseFieldValue[]
            {
                id,
                straatnmid,
                versieid,
                gemeenteid,
                straatnm,
                homoniemtv,
                status
            };
        }
    }
}
