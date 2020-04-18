//using System;
//using System.IO;
//using System.Security.Cryptography;
//using System.Xml.Serialization;
//using LuxEngine.ECS;

//namespace LuxEngine.Systems
//{
//    public class RSAKeyPair : AComponent<RSAKeyPair>
//    {
//        public RSAParameters PrivateKey;
//        public RSAParameters PublicKey;
//    }

//    public class RSAKeysGeneratorSystem : ASystem<RSAKeysGeneratorSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<RSAKeyPair>();
//        }

//        public override void InitSingleton()
//        {
//            AddSingletonComponent(new RSAKeyPair());
//        }

//        protected override void OnRegisterEntity(Entity entity)
//        {
//            // Generate RSA key pair
//            var csp = new RSACryptoServiceProvider(2048)
//            {
//                PersistKeyInCsp = false
//            };

//            // Update the key pair component
//            Unpack(entity, out RSAKeyPair keyPairComponent);
//            keyPairComponent.PrivateKey = csp.ExportParameters(true);
//            keyPairComponent.PublicKey = csp.ExportParameters(false);

//            ////converting the public key into a string representation
//            //string pubKeyString;
//            //{
//            //    using (var sw = new StringWriter())
//            //    {
//            //        var xs = new XmlSerializer(typeof(RSAParameters));

//            //        //serialize the key into the stream
//            //        xs.Serialize(sw, pubKey);
//            //        //get the string from the stream
//            //        pubKeyString = sw.ToString();
//            //    }
//            //}

//            ////converting it back
//            //{
//            //    //get a stream from the string
//            //    var sr = new StringReader(pubKeyString);
//            //    //we need a deserializer
//            //    var xs = new XmlSerializer(typeof(RSAParameters));
//            //    //get the object back from the stream
//            //    pubKey = (RSAParameters)xs.Deserialize(sr);
//            //}

//            ////conversion for the private key is no black magic either ... omitted

//            ////we have a public key ... let's get a new csp and load that key
//            //csp = new RSACryptoServiceProvider();
//            //csp.ImportParameters(pubKey);

//            ////we need some data to encrypt
//            //var plainTextData = "foobar";

//            ////for encryption, always handle bytes...
//            //var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

//            ////apply pkcs#1.5 padding and encrypt our data 
//            //var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

//            ////we might want a string representation of our cypher text... base64 will do
//            //var cypherText = Convert.ToBase64String(bytesCypherText);


//            ///*
//            // * some transmission / storage / retrieval
//            // * 
//            // * and we want to decrypt our cypherText
//            // */

//            ////first, get our bytes back from the base64 string ...
//            //bytesCypherText = Convert.FromBase64String(cypherText);

//            ////we want to decrypt, therefore we need a csp and load our private key
//            //csp = new RSACryptoServiceProvider();
//            //csp.ImportParameters(privKey);

//            ////decrypt and strip pkcs#1.5 padding
//            //bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

//            ////get our original plainText back...
//            //plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
//        }
//    }
//}
