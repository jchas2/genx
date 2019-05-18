<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<!-- Mandatory parameter from the genx cli -->
<xsl:param name="EntityName" />

<!-- Custom parameters -->
<xsl:param name="OrganisationName" />
<xsl:param name="ApplicationName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" />
</xsl:template>

<xsl:template match="md:entitycolumns">
<xsl:variable name="EntityName" select="ancestor::md:entity/@name"/>
<xsl:variable name="EntityOriginalName" select="ancestor::md:entity/@originalname"/>
using <xsl:value-of select="$OrganisationName"/>.Core.Entities;
using <xsl:value-of select="$OrganisationName"/>.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace <xsl:value-of select="$ApplicationName"/>.Core.Entities
{
    [Table("<xsl:value-of select="$EntityOriginalName"/>", Schema = "dbo")]	
    public class <xsl:value-of select="$EntityName"/> : BaseEntity
    {
<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='True'">
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // TODO: Verify
		public <xsl:call-template name="sqltocsharp"><xsl:with-param name="entitycolumn" select="."/></xsl:call-template><xsl:text> </xsl:text><xsl:value-of select="@Name"/> { get; set; }
</xsl:if></xsl:for-each>
<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
<xsl:if test="@allownulls!='true'">
		[Required]</xsl:if>
<xsl:if test="@maxlength!='-1'">
        [MaxLength(<xsl:value-of select="@maxlength"/>, ErrorMessageResourceName = "ERR_MAX_LENGTH_EXCEEDED", ErrorMessageResourceType = typeof(FrameworkMessages))]</xsl:if>
		public <xsl:call-template name="sqltocsharp"><xsl:with-param name="entitycolumn" select="."/></xsl:call-template><xsl:if test="@datatype!='nvarchar'"><xsl:if test="@allownulls='true'">?</xsl:if></xsl:if><xsl:text> </xsl:text><xsl:value-of select="@name"/> { get; set; }
</xsl:if><xsl:for-each select="md:foreignkeys/md:foreignkey">
        [ForeignKey("<xsl:value-of select="@name"/>")]
        public <xsl:value-of select="@primarykeyentity"/><xsl:text> </xsl:text><xsl:value-of select="@primarykeyentity"/> { get; set; }
</xsl:for-each>
</xsl:for-each>

</xsl:template>

<xsl:template match="md:relationships">
<xsl:for-each select="md:relationship">
        public ICollection&lt;<xsl:value-of select="@foreignkeyentity"/>&gt; <xsl:value-of select="@foreignkeyentity"/>s { get; set; }
</xsl:for-each>
	}
}		
</xsl:template>

<xsl:template name="sqltocsharp">
	<xsl:param name="entitycolumn"/>
	<xsl:choose>
		<xsl:when test="@datatype='bigint'">long</xsl:when>
		<xsl:when test="@datatype='binary'">byte[]</xsl:when>
		<xsl:when test="@datatype='bit'">bool</xsl:when>
		<xsl:when test="@datatype='char'">string</xsl:when>
		<xsl:when test="@datatype='datetime'">DateTime</xsl:when>
		<xsl:when test="@datatype='decimal'">decimal</xsl:when>
		<xsl:when test="@datatype='float'">double</xsl:when>
		<xsl:when test="@datatype='int'">int</xsl:when>
		<xsl:when test="@datatype='image'">byte[]</xsl:when>
		<xsl:when test="@datatype='money'">decimal</xsl:when>
		<xsl:when test="@datatype='nchar'">string</xsl:when>
		<xsl:when test="@datatype='numeric'">decimal</xsl:when>
		<xsl:when test="@datatype='ntext'">string</xsl:when>
		<xsl:when test="@datatype='nvarchar'">string</xsl:when>
		<xsl:when test="@datatype='real'">Single</xsl:when>
		<xsl:when test="@datatype='smallint'">short</xsl:when>
		<xsl:when test="@datatype='text'">string</xsl:when>
		<xsl:when test="@datatype='tinyint'">byte</xsl:when>
		<xsl:when test="@datatype='uniqueidentifier'">Guid</xsl:when>		
		<xsl:when test="@datatype='varbinary'">byte[]</xsl:when>		
		<xsl:when test="@datatype='varchar'">string</xsl:when>		
	</xsl:choose>
</xsl:template>

</xsl:stylesheet> 
