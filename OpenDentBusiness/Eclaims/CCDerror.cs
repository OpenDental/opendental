using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Resources;

namespace OpenDentBusiness.Eclaims {
	public class CCDerror {

		///<summary>Get the version 4 error message for the given CDA error code.</summary>
		public static string message(int errorCode,bool useFrench){
			if(useFrench){
				return MessageFr4(errorCode);
			}
			return MessageEn4(errorCode);
		}

		//English error messages for version 4.
		private static string MessageEn4(int errorCode){
			switch(errorCode){
				default:
					break;
				case 1:
					return "Missing/Invalid Transaction Prefix";
				case 2:
					return "Missing/Invalid Dental Claim # or Office Sequence #";
				case 3:
					return "Missing/Invalid Version Number";
				case 4:
					return "Missing/Invalid Transaction Code";
				case 5:
					return "Missing/Invalid Carrier Identification Number";
				case 6:
					return "Missing/Invalid Software System ID";
				case 7:
					return "Missing/Invalid Dentist Unique ID (Provider Number)";
				case 8:
					return "Missing/Invalid Dental Office Number";
				case 9:
					return "Missing/Invalid Primary Policy/Plan Number";
				case 10:
					return "Missing/Invalid Division/Section Number";
				case 11:
					return "Missing/Invalid Subscriber Identification Number";
				case 12:
					return "Missing/Invalid Relationship Code";
				case 13:
					return "Missing/Invalid Patient's Sex";
				case 14:
					return "Missing/Invalid Patient's Birthday";
				case 15:
					return "Missing Patient's Last Name";
				case 16:
					return "Missing Patient's First Name";
				case 17:
					return "Missing/Invalid Eligibility Exception Code";
				case 18:
					return "Missing Name of School";
				case 19:
					return "Missing Subscriber's Last Name or Name did not match to the one on file";
				case 20:
					return "Missing Subscriber's First Name or Name did not match to the one on file";
				case 21:
					return "Missing Subscriber's Address";
				case 22:
					return "Missing Subscriber's City";
				case 23:
					return "Missing/Invalid Subscriber's Postal Code";
				case 24:
					return "Invalid Language of Insured";
				case 25:
					return "Missing/Invalid Subscriber's Birthday";
				case 26:
					return "Invalid Secondary Carrier ID Number";
				case 27:
					return "Missing/Invalid Secondary Policy/Plan Number";
				case 28:
					return "Missing/Invalid Secondary Division/Section Number";
				case 29:
					return "Missing/Invalid Secondary Plan Subscriber Number";
				case 30:
					return "Missing/Invalid Secondary Subscriber's Birthday";
				case 31:
					return "Claim should be submitted to the Secondary Carrier first. (The secondary is the primary carrier)";
				case 32:
					return "Missing/Invalid Payee";
				case 33:
					return "Invalid Accident Date";
				case 34:
					return "Missing/Invalid Number of Procedures Performed";
				case 35:
					return "Missing/Invalid Procedure Code";
				case 36:
					return "Missing/Invalid Date of Service";
				case 37:
					return "Missing/Invalid International Tooth, Sextant, Quadrant or Arch Designation";
				case 38:
					return "Missing/Invalid Tooth Surface";
				case 39:
					return "Invalid Date of Initial Placement (Upper )";
				case 40:
					return "Missing/Invalid Response re: Treatment Required for Orthodontic Purposes";
				case 41:
					return "Missing/Invalid Dentist's Fee Claimed";
				case 42:
					return "Missing/Invalid Lab Fee";
				case 43:
					return "Missing/Invalid Units of Time";
				case 44:
					return "Message Length Field did not match length of message received";
				case 45:
					return "Missing/Invalid E-Mail / Materials Forwarded Flag";
				case 46:
					return "Missing/Invalid Claim Reference Number";
				case 47:
					return "Provider is not Authorized to access CDAnet";
				case 48:
					return "Please Submit Claim Manually";
				case 49:
					return "No outstanding responses from the network requested";
				case 50:
					return "Missing/Invalid Procedure Line Number";
				case 51:
					return "Predetermination number not found";
				case 52:
					return "At least one service must be entered for a claim/predetermination";
				case 53:
					return "Missing/Invalid Subscriber's province";
				case 54:
					return "Subscriber ID on reversal did not match that on the original claim";
				case 55:
					return "Reversal not for today's transaction";
				case 56:
					return "Provider's specialty code does not match that on file";
				case 57:
					return "Missing/Invalid response to Question re: Is this an initial placement (Upper)";
				case 58:
					return "Number of procedures found did not match with number indicated";
				case 59:
					return "Dental Office Software is not certified to submit transactions to CDAnet";
				case 60:
					return "Claim Reversal Transaction cannot be accepted now, please try again later today";
				case 61:
					return "Network error, please re-submit transaction";
				case 62:
					return "Missing/Invalid Payee CDA Provider Number";
				case 63:
					return "Missing/Invalid Payee Provider Office Number";
				case 64:
					return "Missing/Invalid Referring Provider";
				case 65:
					return "Missing/Invalid Referral Reason Code";
				case 66:
					return "Missing/Invalid Plan Flag";
				case 67:
					return "Missing NIHB Plan fields";
				case 68:
					return "Missing/Invalid Band Number";
				case 69:
					return "Missing/Invalid Family Number";
				case 70:
					return "Missing/Invalid Missing Teeth Map";
				case 71:
					return "Missing/Invalid Secondary Relationship Code";
				case 72:
					return "Missing/Invalid Procedure Type Codes";
				case 73:
					return "Missing/Invalid Remarks Code";
				case 74:
					return "Date of Service is a future date";
				case 75:
					return "Date of Service is more than one week old";
				case 76:
					return "Group not acceptable through EDI";
				case 77:
					return "Procedure Type not supported by carrier";
				case 78:
					return "Please submit pre-authorization manually";
				case 79:
					return "Duplicate Claim";
				case 80:
					return "Missing/Invalid Carrier Transaction Counter";
				case 81:
					return "Invalid Eligibility Date";
				case 82:
					return "Invalid Card Sequence/Version Number";
				case 83:
					return "Missing/Invalid Secondary Subscriber's Last Name";
				case 84:
					return "Missing/Invalid Secondary Subscriber's First Name";
				case 85:
					return "Invalid Secondary Subscriber's Middle Initial";
				case 86:
					return "Missing Secondary Subscriber's Address Line 1";
				case 87:
					return "Missing Secondary Subscriber's City";
				case 88:
					return "Missing Secondary Subscriber's Province/State Code";
				case 89:
					return "Invalid Secondary Subscriber's Postal/Zip Code";
				case 90:
					return "Missing/Invalid response to Question: Is this an Initial Placement Lower";
				case 91:
					return "Missing/Invalid Date of Initial Placement Lower";
				case 92:
					return "Missing/Invalid Maxillary Prosthesis Material";
				case 93:
					return "Missing/Invalid Mandibular Prosthesis Material";
				case 94:
					return "Missing/Invalid Extracted Teeth Count";
				case 95:
					return "Missing/Invalid Extracted Tooth Number";
				case 96:
					return "Missing/Invalid Extraction Date";
				case 97:
					return "Invalid Reconciliation Date";
				case 98:
					return "Missing/Invalid Lab Procedure Code";
				case 99:
					return "Invalid Encryption Code";
				case 100:
					return "Invalid Encryption";
				case 101:
					return "Invalid Subscriber's Middle Initial";
				case 102:
					return "Invalid Patient’s Middle Initial";
				case 103:
					return "Missing/Invalid Primary Dependant Code";
				case 104:
					return "Missing/Invalid Secondary Dependant Code";
				case 105:
					return "Missing/Invalid Secondary Card Sequence/Version Number";
				case 106:
					return "Missing/Invalid Secondary Language";
				case 107:
					return "Missing/Invalid Secondary Coverage Flag";
				case 108:
					return "Secondary Coverage Fields Missing";
				case 109:
					return "Missing/Invalid Secondary Sequence Number";
				case 110:
					return "Missing/Invalid Orthodontic Record Flag";
				case 111:
					return "Missing/Invalid First Examination Fee";
				case 112:
					return "Missing/Invalid Diagnostic Phase Fee";
				case 113:
					return "Missing/Invalid Initial Payment";
				case 114:
					return "Missing/Invalid Payment Mode";
				case 115:
					return "Missing/Invalid Treatment Duration";
				case 116:
					return "Missing/Invalid Number of Anticipated Payments";
				case 117:
					return "Missing/Invalid Anticipated Payment Amount";
				case 118:
					return "Missing/Invalid Lab Procedure Code # 2";
				case 119:
					return "Missing/Invalid Lab Procedure Fee # 2";
				case 120:
					return "Missing/Invalid Estimated Treatment Starting Date";
				case 121:
					return "Primary EOB Altered from the Original";
				case 122:
					return "Data no longer available";
				case 123:
					return "Missing/Invalid Reconciliation Page Number";
				case 124:
					return "Transaction Type not supported by the carrier";
				case 125:
					return "Transaction Version not supported";
				case 126:
					return "Missing/Invalid Diagnostic Code";
				case 127:
					return "Missing/Invalid Institution Code";
				case 128:
					return "Missing/Invalid Current Predetermination Page Number";
				case 129:
					return "Missing/Invalid Last Predetermination Page Number";
				case 130:
					return "Missing/Invalid Plan Record Count";
				case 131:
					return "Missing/Invalid Plan Record";
				case 132:
					return "Missing/Invalid Secondary Record Count";
				case 133:
					return "Missing/Invalid Embedded Transaction Length";
				case 134:
					return "Invalid Secondary Address Line # 2";
				case 135:
					return "Missing / Invalid Receiving Provider Number";
				case 136:
					return "Missing / Invalid Receiving Office Number";
				case 137:
					return "Missing / Invalid Original Office Sequence Number";
				case 138:
					return "Missing / Invalid Original Transaction Reference Number";
				case 139:
					return "Missing / Invalid Attachment Source";
				case 140:
					return "Missing / Invalid Attachment Count";
				case 141:
					return "Missing / Invalid Attachment Type";
				case 142:
					return "Missing / Invalid Attachment Length";
				case 143:
					return "Missing / Invalid Attachment";
				case 144:
					return "Missing / Invalid Attachment File Date";
				case 145:
					return "Submitted Claim’s Predetermination number indicates claim must be made manually";
				case 146:
					return "Submitted Claim’s Predetermination number has expired";
				case 147:
					return "Overage dependant is not a student or disabled";
				case 148:
					return "Subscriber does not have dental coverage";
				case 149:
					return "Patient is not eligible";
				case 150:
					return "Lab bill is not allowed";
				case 151:
					return "Patient’s name / birth year does not match our files";
				case 152:
					return "Lab bill must be submitted on the same line as the associated professional fee";
				case 153:
					return "Our records indicate another payor should be primary";
				case 997:
					return "Last Transaction Unreadable";
				case 998:
					return "Reserved by CDAnet for future use";
				case 999:
					return "Host Processing Error - Resubmit Claim Manually";
			}
			return "UNKNOWN ERROR";
		}

		//French error messages for version 4.
		private static string MessageFr4(int errorCode){
			switch(errorCode){
				default:
					break;
				case 1:
					return "Préfixe de la transaction absent ou invalide";
				case 2:
					return "Numéro de demande de prestations ou de transaction du cabinet absent ou invalide";
				case 3:
					return "Numéro de version absent ou invalide";
				case 4:
					return "Code de la transaction absent ou invalide";
				case 5:
					return "Numéro de l'assureur absent ou invalide";
				case 6:
					return "Numéro du logiciel dentaire absent ou invalide";
				case 7:
					return "Numéro du dentiste attribué par l'ADC absent ou invalide";
				case 8:
					return "Numéro du cabinet attribué par l'ADC absent ou invalide";
				case 9:
					return "Numéro de police ou régime (premier assureur) absent ou invalide";
				case 10:
					return "Numéro de section ou de division absent ou invalide";
				case 11:
					return "Numéro du titulaire de l'assurance absent ou invalide";
				case 12:
					return "Code indiquant lien de parenté patient-titulaire absent ou invalide";
				case 13:
					return "Sexe du patient absent ou invalide";
				case 14:
					return "Date de naissance du patient absente ou invalide";
				case 15:
					return "Nom de famille du patient absent ou invalide";
				case 16:
					return "Prénom du patient absent ou invalide";
				case 17:
					return "Code indiquant exception quant à l'admissibilité absent ou invalide";
				case 18:
					return "Nom de l'établissement scolaire absent";
				case 19:
					return "Nom de famille du titulaire de l'assurance absent ou non conforme au dossier";
				case 20:
					return "Prénom du titulaire de l'assurance absent ou non conforme au dossier";
				case 21:
					return "Adresse du titulaire de l'assurance absente";
				case 22:
					return "Ville du titulaire de l'assurance absente";
				case 23:
					return "Code postal du titulaire de l'assurance absent ou invalide";
				case 24:
					return "Langue première du titulaire de l'assurance invalide";
				case 25:
					return "Date de naissance du titulaire de l'assurance absente ou invalide";
				case 26:
					return "Numéro du second assureur invalide";
				case 27:
					return "Numéro de police ou régime (second assureur) absent ou invalide";
				case 28:
					return "Numéro de division ou section (second assureur) absent ou invalide";
				case 29:
					return "Numéro du titulaire (second assureur) absent ou invalide";
				case 30:
					return "Date de naissance du titulaire (second assureur) absente ou invalide";
				case 31:
					return "Demande doit d'abord être soumise au second assureur (second assureur = premier assureur)";
				case 32:
					return "Destinataire du paiement absent ou invalide";
				case 33:
					return "Date de l'accident invalide";
				case 34:
					return "Nombre d'actes exécutés absent ou invalide";
				case 35:
					return "Code de l'acte absent ou invalide";
				case 36:
					return "Date à laquelle l'acte a été exécuté absente ou invalide";
				case 37:
					return "Numéro de dent international, sextant, quadrant ou site absent ou invalide";
				case 38:
					return "Surface de la dent absente ou invalide";
				case 39:
					return "Date de la mise en bouche initiale au maxillaire invalide";
				case 40:
					return "Réponse absente ou invalide : Le traitement est-il requis en vue de soins d'orthodontie ?";
				case 41:
					return "Honoraires demandés par le dentiste absents ou invalides";
				case 42:
					return "Frais de laboratoire absents ou invalides";
				case 43:
					return "Unité de temps absente ou invalide";
				case 44:
					return "Longueur du message indiquée non identique à longueur du message reçu";
				case 45:
					return "Indicateur de courrier électronique ou d'informations supplémentaires absent ou invalide";
				case 46:
					return "Numéro de référence de la demande de prestations absent ou invalide";
				case 47:
					return "Dentiste n'a pas accès au réseau CDAnet";
				case 48:
					return "Veuillez soumettre demande manuellement";
				case 49:
					return "Pas de réponse en suspens provenant du réseau demandée";
				case 50:
					return "Numéro de ligne de l'acte absent ou invalide";
				case 51:
					return "Numéro du plan de traitement introuvable";
				case 52:
					return "Demande de prestations ou plan de traitement doit contenir au moins un acte";
				case 53:
					return "Province du titulaire de l'assurance absente ou invalide";
				case 54:
					return "Numéro du titulaire sur Refus non conforme à demande originale";
				case 55:
					return "Annulation ne concerne pas transaction du jour";
				case 56:
					return "Code de spécialité du dentiste non conforme au dossier";
				case 57:
					return "Réponse absente ou invalide : S'agit-il de la mise en bouche initiale au maxillaire ?";
				case 58:
					return "Nombre d'actes non conforme au nombre indiqué";
				case 59:
					return "Logiciel dentaire du cabinet non autorisé à transmettre transactions au CDAnet";
				case 60:
					return "Annulation ne peut être acceptée maintenant - Réessayer plus tard aujourd'hui";
				case 61:
					return "Erreur du réseau - Veuillez recommencer";
				case 62:
					return "Numéro du dentiste destinataire du paiement absent ou invalide";
				case 63:
					return "Numéro du cabinet destinataire du paiement absent ou invalide";
				case 64:
					return "Dentiste ayant adressé (référé) patient absent ou invalide";
				case 65:
					return "Code indiquant motif de la recommandation absent ou invalide";
				case 66:
					return "Indicateur d'un régime de soins absent ou invalide";
				case 67:
					return "Champs se rapportant au régime NNSA absents";
				case 68:
					return "Numéro de la bande absent ou invalide";
				case 69:
					return "Numéro de la famille absent ou invalide";
				case 70:
					return "Odontogramme des dents manquantes absent ou invalide";
				case 71:
					return "Code indiquant parenté patient-titulaire (second assureur) absent ou invalide";
				case 72:
					return "Code indiquant type d'acte absent ou invalide";
				case 73:
					return "Série de dents absente ou invalide";
				case 74:
					return "Date à laquelle l'acte a été exécuté est une date ultérieure";
				case 75:
					return "Date à laquelle l'acte a été exécuté est au-delà d'un an";
				case 76:
					return "Groupe non accepté par l'EDI";
				case 77:
					return "Type d'acte non couvert par l'assureur";
				case 78:
					return "Veuillez soumettre plan de traitement manuellement";
				case 79:
					return "Duplicata d'une demande de prestations";
				case 80:
					return "Compteur des transactions par assureur absent ou invalide";
				case 81:
					return "Date d'admissibilité invalide";
				case 82:
					return "Numéro de séquence ou version de la carte invalide";
				case 83:
					return "Nom de famille du titulaire (second assureur) absent ou invalide";
				case 84:
					return "Prénom du titulaire (second assureur) absent ou invalide";
				case 85:
					return "Lettre initiale du second prénom du titulaire (second assureur) invalide";
				case 86:
					return "Première ligne de l'adresse du titulaire (second assureur) absente";
				case 87:
					return "Ville du titulaire (second assureur) absente";
				case 88:
					return "Province ou État du titulaire (second assureur) absent";
				case 89:
					return "Code postal ou zip du titulaire (second assureur) invalide";
				case 90:
					return "Réponse absente ou invalide : S'agit-il de la mise en bouche initiale à la mandibule ?";
				case 91:
					return "Date de la mise en bouche initiale à la mandibule absente ou invalide";
				case 92:
					return "Matériau de la prothèse au maxillaire absent ou invalide";
				case 93:
					return "Matériau de la prothèse à la mandibule absent ou invalide";
				case 94:
					return "Nombre de dents extraites absent ou invalide";
				case 95:
					return "Numéro de la dent extraite absent ou invalide";
				case 96:
					return "Date de l'extraction absente ou invalide";
				case 97:
					return "Décalage du rapprochement invalide";
				case 98:
					return "Code pour frais de laboratoire absent ou invalide";
				case 99:
					return "Code pour chiffrement invalide";
				case 100:
					return "Chiffrement invalide";
				case 101:
					return "Initiale du second prénom du titulaire invalide";
				case 102:
					return "Initiale du second prénom du patient invalide";
				case 103:
					return "Code de la personne à charge (première assurance) absent ou invalide";
				case 104:
					return "Code de la personne à charge (seconde assurance) absent ou invalide";
				case 105:
					return "Numéro de séquence/version de la carte (seconde assurance) absent ou invalide";
				case 106:
					return "Langue de titulaire (seconde assurance) absente ou invalide";
				case 107:
					return "Indicateur de régime (seconde assurance) absente ou invalide";
				case 108:
					return "Champs portant sur la seconde assurance absents";
				case 109:
					return "Numéro de séquence (seconde assurance) absent ou invalide";
				case 110:
					return "Indicateur de Plan de traitement d’orthodontie absent ou invalide";
				case 111:
					return "Tarif du premier examen absent ou invalide";
				case 112:
					return "Tarif de la phase diagnostique absent ou invalide";
				case 113:
					return "Paiement initial absent ou invalide";
				case 114:
					return "Mode de paiement absent ou invalide";
				case 115:
					return "Durée du traitement absente ou invalide";
				case 116:
					return "Nombre prévu de paiements absent ou invalide";
				case 117:
					return "Montant prévue du paiement absent ou invalide";
				case 118:
					return "Code des frais de laboratoire #2 absent ou invalide";
				case 119:
					return "Frais de laboratoire #2 absents ou invalide";
				case 120:
					return "Début prévue de traitement";
				case 121:
					return "Détail des prestations (première assurance) modifié, différent de l’original";
				case 122:
					return "Date plus disponible";
				case 123:
					return "Numéro de page du rapprochement absent ou invalide";
				case 124:
					return "Transaction non acceptée par l’assureur";
				case 125:
					return "Version de transaction non acceptée";
				case 126:
					return "Code diagnostique absent ou invalide";
				case 127:
					return "Code institutionnelle absent ou invalide";
				case 128:
					return "Numéro de page du plan de traitement courant absent ou invalide";
				case 129:
					return "Numéro de page du dernier plan de traitement absent ou invalide";
				case 130:
					return "Nombre du plan du dossier gouvernemental absent ou invalide";
				case 131:
					return "Plan du dossier gouvernemental absent ou invalide";
				case 132:
					return "Nombre du dossier secondaire absent ou invalide";
				case 133:
					return "";
				case 134:
					return "";
				case 997:
					return "Dernière transaction illisible";
				case 998:
					return "Pour usage futur par l'ADC";
				case 999:
					return "Erreur du système central - Veuillez recommencer manuellement";
			}
			return "CODE D'ERREUR INCONNU";//UNKNOWN ERROR
		}

	}
}
