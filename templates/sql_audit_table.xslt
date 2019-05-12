<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<!-- Mandatory parameter from the genx cli -->
<xsl:param name="EntityName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
</xsl:template>

<xsl:template match="md:entitycolumns">
<xsl:variable name="EntityName" select="ancestor::md:entity/@name"/>
<xsl:variable name="EntityOriginalName" select="ancestor::md:entity/@originalname"/>

CREATE TABLE [dbo].[<xsl:value-of select="$EntityOriginalName"/>History](
    [HistoryId] [int] IDENTITY(1,1) NOT NULL,<xsl:for-each select="md:column">
	[<xsl:value-of select="@originalname"/>] [<xsl:value-of select="@datatype"/>]<xsl:if test="@datatype='char' or @datatype='nchar' or @datatype='nvarchar' or @datatype='varchar'">(<xsl:value-of select="@maxlength"/>)</xsl:if><xsl:if test="@allownulls!='true'"> NOT NULL,</xsl:if><xsl:if test="@allownulls='true'"> NULL,</xsl:if>
</xsl:for-each>
	[AuditId] [int] NOT NULL,
	[AuditAction] [int] NOT NULL,    
CONSTRAINT [PK_<xsl:value-of select="$EntityOriginalName"/>History] PRIMARY KEY CLUSTERED 
(
	[HistoryId] ASC
) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
ON [PRIMARY]

ALTER TABLE [dbo].[<xsl:value-of select="$EntityOriginalName"/>History]  WITH CHECK ADD  CONSTRAINT [FK_<xsl:value-of select="$EntityOriginalName"/>History_AuditLog] FOREIGN KEY([AuditId])
REFERENCES [dbo].[AuditLog] ([AuditId])

</xsl:template>

</xsl:stylesheet> 
