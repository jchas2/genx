<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<xsl:param name="EntityName" />
<xsl:param name="OrganisationName" />
<xsl:param name="ApplicationName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" />
</xsl:template>

<xsl:template match="dbs:TableColumns">
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
		public <xsl:call-template name="ConvertToCLRType"><xsl:with-param name="TableColumn" select="."/></xsl:call-template><xsl:text> </xsl:text><xsl:value-of select="@Name"/> { get; set; }
</xsl:if></xsl:for-each>
<xsl:for-each select="dbs:TableColumn"><xsl:if test="@IsPrimaryKey='False'">
<xsl:if test="@AllowNulls!='true'">
		[Required]</xsl:if>
<xsl:if test="@MaxLength!=''">
        [MaxLength(<xsl:value-of select="@MaxLength"/>, ErrorMessageResourceName = "ERR_MAX_LENGTH_EXCEEDED", ErrorMessageResourceType = typeof(FrameworkMessages))]</xsl:if>
		public <xsl:call-template name="ConvertToCLRType"><xsl:with-param name="TableColumn" select="."/></xsl:call-template><xsl:if test="@datatype!='nvarchar'"><xsl:if test="@allownulls='true'">?</xsl:if></xsl:if><xsl:text> </xsl:text><xsl:value-of select="@name"/> { get; set; }
</xsl:if><xsl:for-each select="md:foreignkeys/md:foreignkey">
        [ForeignKey("<xsl:value-of select="@Name"/>")]
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

<xsl:template name="ConvertToCLRType">
	<xsl:param name="TableColumn"/>
	<xsl:choose>
		<xsl:when test="@SQLType='Integer'">int</xsl:when>
		<xsl:when test="@SQLType='Numeric'">double</xsl:when>
		<xsl:when test="@SQLType='Binary'">byte[]</xsl:when>
		<xsl:when test="@SQLType='Boolean'">bool</xsl:when>
		<xsl:when test="@SQLType='Double'">double</xsl:when>
		<xsl:when test="@SQLType='Date'">DateTime</xsl:when>
		<xsl:when test="@SQLType='DBTimeStamp'">DateTime</xsl:when>
		<xsl:when test="@SQLType='Currency'">float</xsl:when>
		<xsl:when test="@SQLType='TinyInt'">byte</xsl:when>
		<xsl:when test="@SQLType='UnsignedTinyInt'">byte</xsl:when>
		<xsl:when test="@SQLType='Variant'">object</xsl:when>
		<xsl:when test="@SQLType='VarChar'">string</xsl:when>
		<xsl:when test="@SQLType='Char'">string</xsl:when>
		<xsl:when test="@SQLType='WChar'">string</xsl:when>		
	</xsl:choose>
</xsl:template>


</xsl:stylesheet> 
